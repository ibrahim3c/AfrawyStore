using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Enums;
using System.Globalization;

namespace AfrawyStore.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;

    public DashboardService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var today = DateTime.Today;
        var sevenDaysAgo = today.AddDays(-6);

        // 1. Fetch data in parallel where possible (or sequential for simplicity in first pass)
        var products = await _uow.Products.FindAsync(p => p.IsActive);
        var categories = await _uow.Categories.GetAllAsync();
        
        var todaySalesList = await _uow.Sales.GetSalesSinceDateAsync(today);
        var recentSalesList = await _uow.Sales.GetSalesSinceDateAsync(sevenDaysAgo);
        
        var latestSalesEntities = await _uow.Sales.GetLatestSalesAsync(5);
        var lowStockEntities = await _uow.Inventory.GetLowStockItemsAsync();
        var lowStockCount = await _uow.Inventory.GetLowStockCountAsync();

        var model = new DashboardViewModel
        {
            TotalProducts = products.Count(),
            TotalCategories = categories.Count(),
            TodaySales = todaySalesList.Sum(s => s.TotalAmount),
            TodaySalesCount = todaySalesList.Count,
            LowStockCount = lowStockCount
        };

        // 2. Process Chart Data (Last 7 Days)
        var arabicCulture = new CultureInfo("ar-EG");
        for (int i = 0; i < 7; i++)
        {
            var date = sevenDaysAgo.AddDays(i);
            var dayTotal = recentSalesList
                .Where(s => s.SaleDate.Date == date.Date)
                .Sum(s => s.TotalAmount);

            model.Last7DaysSales.Add(new DailySalesDto
            {
                Date = date,
                DayLabel = date.ToString("dddd", arabicCulture),
                Total = dayTotal
            });
        }

        // 3. Map Latest Sales
        model.LatestSales = latestSalesEntities.Select(s => new SaleDto
        {
            Id = s.Id,
            SaleDate = s.SaleDate,
            TotalAmount = s.TotalAmount,
            TotalProfit = s.TotalProfit,
            Discount = s.Discount,
            
            Status = s.Status,
            CreatedByName = s.CreatedBy?.FullName ?? "Unknown",
            ItemCount = s.SaleItems.Count,
            Note = s.Note
        }).ToList();

        // 4. Map Low Stock Items
        model.LowStockItems = lowStockEntities.Take(5).Select(i => new LowStockItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            SKU = i.Product.SKU,
            CurrentStock = i.CurrentStock,
            MinimumStock = i.Product.MinimumStock,
            Unit = i.Product.Unit
        }).ToList();

        return model;
    }
}
