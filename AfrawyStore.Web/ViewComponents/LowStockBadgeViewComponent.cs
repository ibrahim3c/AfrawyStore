using AfrawyStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.ViewComponents;

public class LowStockBadgeViewComponent : ViewComponent
{
    private readonly IInventoryService _inventoryService;

    public LowStockBadgeViewComponent(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = await _inventoryService.GetLowStockCountAsync();
        return View(count);
    }
}
