using System.Security.Claims;
using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

[Authorize]
public class SalesController : Controller
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    /// <summary>
    /// Sales list with filters and pagination (5.13)
    /// </summary>
    public async Task<IActionResult> Index(string? searchTerm, DateTime? dateFrom, DateTime? dateTo, string? status, int page = 1)
    {
        SaleStatus? statusFilter = status switch
        {
            "completed" => SaleStatus.Completed,
            "voided" => SaleStatus.Voided,
            _ => null
        };

        var filter = new SalesFilterDto
        {
            SearchTerm = searchTerm,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Status = statusFilter,
            Page = page,
            PageSize = 20
        };

        var (sales, totalCount) = await _saleService.GetPagedSalesAsync(filter);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
        ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
        ViewBag.Status = status;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / 20);
        ViewBag.TotalCount = totalCount;

        return View(sales);
    }

    /// <summary>
    /// POS view — new sale (5.4)
    /// </summary>
    public IActionResult New()
    {
        return View();
    }

    /// <summary>
    /// AJAX endpoint: search products for POS (5.5)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SearchProducts(string? term)
    {
        var products = await _saleService.SearchProductsForPosAsync(term);
        return Json(products);
    }

    /// <summary>
    /// AJAX endpoint: process checkout (5.6–5.11)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout([FromBody] CreateSaleDto model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, error = string.Join(" | ", errors) });
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Json(new { success = false, error = "خطأ في تحديد هوية المستخدم." });
        }

        var (saleId, error) = await _saleService.CreateSaleAsync(model, userId);

        if (saleId.HasValue)
        {
            return Json(new { success = true, saleId = saleId.Value });
        }

        return Json(new { success = false, error });
    }

    /// <summary>
    /// Sale detail / receipt view (5.12)
    /// </summary>
    public async Task<IActionResult> Detail(int id)
    {
        var sale = await _saleService.GetSaleDetailAsync(id);
        if (sale == null) return NotFound();

        ViewBag.IsAdmin = User.IsInRole("Admin");
        return View(sale);
    }

    /// <summary>
    /// Void a sale — Admin only (5.14)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Void(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out int userId))
        {
            TempData["ErrorMessage"] = "خطأ في تحديد هوية المستخدم.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        var error = await _saleService.VoidSaleAsync(id, userId);

        if (error == null)
        {
            TempData["SuccessMessage"] = "تم إلغاء عملية البيع بنجاح وتمت إعادة المخزون.";
        }
        else
        {
            TempData["ErrorMessage"] = error;
        }

        return RedirectToAction(nameof(Detail), new { id });
    }
}
