using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfrawyStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return View(categories);
    }

    public async Task<IActionResult> Create()
    {
        await LoadParentCategoriesAsync();
        return View(new CategoryCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateDto model)
    {
        if (ModelState.IsValid)
        {
            var success = await _categoryService.CreateCategoryAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "تم إضافة الفئة بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "حدث خطأ أثناء حفظ الفئة.");
        }
        await LoadParentCategoriesAsync(model.ParentCategoryId);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        var editDto = new CategoryEditDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId
        };

        await LoadParentCategoriesAsync(category.ParentCategoryId, excludeId: category.Id);
        return View(editDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryEditDto model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var success = await _categoryService.UpdateCategoryAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "تم تعديل الفئة بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "حدث خطأ أثناء تعديل الفئة. تأكد من عدم اختيار الفئة نفسها كفئة رئيسية.");
        }
        
        await LoadParentCategoriesAsync(model.ParentCategoryId, excludeId: model.Id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var errorMessage = await _categoryService.DeleteCategoryAsync(id);
        
        if (string.IsNullOrEmpty(errorMessage))
        {
            TempData["SuccessMessage"] = "تم حذف الفئة بنجاح.";
        }
        else
        {
            TempData["ErrorMessage"] = errorMessage;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadParentCategoriesAsync(int? selectedId = null, int? excludeId = null)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        
        if (excludeId.HasValue)
        {
            // Simple approach: exclude the category itself and its direct children 
            // (a full tree traversal is better for deep hierarchies, but this is usually sufficient for simple setups)
            categories = categories.Where(c => c.Id != excludeId.Value && c.ParentCategoryId != excludeId.Value).ToList();
        }

        ViewBag.ParentCategories = new SelectList(categories, "Id", "Name", selectedId);
    }
}
