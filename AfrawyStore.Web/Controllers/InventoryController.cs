using System.Security.Claims;
using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

[Authorize]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IActionResult> Index(string? filter)
    {
        var inventories = await _inventoryService.GetAllInventoryAsync();

        if (filter == "low")
        {
            inventories = inventories.Where(i => i.StockStatus == "low" || i.StockStatus == "out").ToList();
            ViewBag.Filter = "low";
        }

        return View(inventories);
    }

    public async Task<IActionResult> Adjust(int productId)
    {
        var adjustForm = await _inventoryService.GetAdjustFormAsync(productId);
        if (adjustForm == null) return NotFound();
        return View(adjustForm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adjust(InventoryAdjustDto model)
    {
        if (ModelState.IsValid)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "خطأ في تحديد هوية المستخدم.";
                return RedirectToAction(nameof(Index));
            }

            var errorMessage = await _inventoryService.AdjustStockAsync(model, userId);
            if (string.IsNullOrEmpty(errorMessage))
            {
                TempData["SuccessMessage"] = "تم تحديث المخزون بنجاح.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, errorMessage);
        }

        // Re-populate the read-only fields
        var adjustForm = await _inventoryService.GetAdjustFormAsync(model.ProductId);
        if (adjustForm != null)
        {
            model.ProductName = adjustForm.ProductName;
            model.ProductSKU = adjustForm.ProductSKU;
            model.CurrentStock = adjustForm.CurrentStock;
            model.Unit = adjustForm.Unit;
        }

        return View(model);
    }

    public async Task<IActionResult> Logs(int productId)
    {
        var adjustForm = await _inventoryService.GetAdjustFormAsync(productId);
        if (adjustForm == null) return NotFound();

        var logs = await _inventoryService.GetLogsAsync(productId);

        ViewBag.ProductName = adjustForm.ProductName;
        ViewBag.ProductSKU = adjustForm.ProductSKU;
        ViewBag.ProductId = productId;

        return View(logs);
    }
}
