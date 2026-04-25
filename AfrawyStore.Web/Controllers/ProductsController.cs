using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfrawyStore.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
    {
        _productService = productService;
        _categoryService = categoryService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, bool? isActive, int page = 1)
    {
        const int pageSize = 20;
        var pagedResult = await _productService.GetPagedProductsAsync(searchTerm, categoryId, isActive, page, pageSize);

        // Load categories for filter dropdown
        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        
        // Preserve filter states
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CategoryId = categoryId;
        ViewBag.IsActive = isActive;

        return View(pagedResult);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View(new ProductCreateDto { IsActive = true, CostPrice = 1, SellingPrice = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateDto model, IFormFile? ImageFile)
    {
        if (model.SellingPrice < model.CostPrice)
        {
            ModelState.AddModelError(nameof(model.SellingPrice), "سعر البيع يجب أن يكون أكبر من أو يساوي سعر التكلفة.");
        }

        if (ModelState.IsValid)
        {
            var isUnique = await _productService.IsSkuUniqueAsync(model.SKU);
            if (!isUnique)
            {
                ModelState.AddModelError(nameof(model.SKU), "رمز المنتج (SKU) مستخدم بالفعل.");
            }
            else
            {
                string? imagePath = await ProcessUploadedFile(ImageFile);
                var success = await _productService.CreateProductAsync(model, imagePath);

                if (success)
                {
                    TempData["SuccessMessage"] = "تم إضافة المنتج بنجاح.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء إضافة المنتج.");
            }
        }

        await LoadCategoriesAsync(model.CategoryId);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        var editDto = new ProductEditDto
        {
            Id = product.Id,
            SKU = product.SKU,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice,
            Unit = product.Unit,
            IsActive = product.IsActive,
            ExistingImagePath = product.ImagePath
        };

        await LoadCategoriesAsync(editDto.CategoryId);
        return View(editDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditDto model, IFormFile? ImageFile)
    {
        if (id != model.Id) return NotFound();

        if (model.SellingPrice < model.CostPrice)
        {
            ModelState.AddModelError(nameof(model.SellingPrice), "سعر البيع يجب أن يكون أكبر من أو يساوي سعر التكلفة.");
        }

        if (ModelState.IsValid)
        {
            var isUnique = await _productService.IsSkuUniqueAsync(model.SKU, model.Id);
            if (!isUnique)
            {
                ModelState.AddModelError(nameof(model.SKU), "رمز المنتج (SKU) مستخدم بالفعل.");
            }
            else
            {
                string? newImagePath = model.ExistingImagePath;
                if (ImageFile != null)
                {
                    newImagePath = await ProcessUploadedFile(ImageFile);
                }

                var success = await _productService.UpdateProductAsync(model, newImagePath);
                if (success)
                {
                    TempData["SuccessMessage"] = "تم تعديل المنتج بنجاح.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء تعديل المنتج.");
            }
        }

        await LoadCategoriesAsync(model.CategoryId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "تم حذف المنتج بنجاح (نقل إلى الأرشيف).";
        }
        else
        {
            TempData["ErrorMessage"] = "حدث خطأ أثناء محاولة حذف المنتج.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkToggleStatus(int[] selectedIds, bool isActive)
    {
        if (selectedIds == null || selectedIds.Length == 0)
        {
            TempData["ErrorMessage"] = "يرجى تحديد منتج واحد على الأقل.";
            return RedirectToAction(nameof(Index));
        }

        var success = await _productService.BulkToggleStatusAsync(selectedIds, isActive);
        if (success)
        {
            TempData["SuccessMessage"] = $"تم تغيير حالة {selectedIds.Length} منتج بنجاح.";
        }
        else
        {
            TempData["ErrorMessage"] = "حدث خطأ أثناء تغيير حالة المنتجات.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategoriesAsync(int? selectedId = null)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
    }

    private async Task<string?> ProcessUploadedFile(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/uploads/products/{uniqueFileName}";
    }
}
