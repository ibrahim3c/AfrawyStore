using System.Globalization;
using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;

namespace AfrawyStore.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    private static readonly CultureInfo ArabicCulture = new("ar-EG");

    public ReportService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // ─────────────────────────────────────────────────────────
    // 7.4  Sales Report
    // ─────────────────────────────────────────────────────────
    public async Task<SalesReportDto> GetSalesReportAsync(ReportFilterDto filter)
    {
        // Default date range: last 30 days if not specified
        var dateFrom = filter.DateFrom ?? DateTime.Today.AddDays(-29);
        var dateTo   = filter.DateTo   ?? DateTime.Today;

        var sales = await _uow.Sales.GetSalesForReportAsync(dateFrom, dateTo);

        var report = new SalesReportDto
        {
            TotalSalesCount = sales.Count,
            TotalRevenue    = sales.Sum(s => s.TotalAmount),
            TotalProfit     = sales.Sum(s => s.TotalProfit),
            TotalDiscount   = sales.Sum(s => s.Discount),
            CashCount       = sales.Count(s => s.PaymentMethod == Domain.Enums.PaymentMethod.Cash),
            CardCount       = sales.Count(s => s.PaymentMethod == Domain.Enums.PaymentMethod.Card),
            OtherCount      = sales.Count(s => s.PaymentMethod == Domain.Enums.PaymentMethod.Other),
        };

        report.AverageSaleValue = report.TotalSalesCount > 0
            ? Math.Round(report.TotalRevenue / report.TotalSalesCount, 2)
            : 0;

        // Build a row per calendar day in the range (even days with zero sales)
        for (var day = dateFrom.Date; day <= dateTo.Date; day = day.AddDays(1))
        {
            var daySales = sales.Where(s => s.SaleDate.Date == day).ToList();
            report.DailyRows.Add(new SalesReportRowDto
            {
                Date      = day,
                DateLabel = day.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                SaleCount = daySales.Count,
                Revenue   = daySales.Sum(s => s.TotalAmount),
                Profit    = daySales.Sum(s => s.TotalProfit),
                Discount  = daySales.Sum(s => s.Discount),
            });
        }

        return report;
    }

    // ─────────────────────────────────────────────────────────
    // 7.5  Inventory Status Report
    // ─────────────────────────────────────────────────────────
    public async Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync()
    {
        var inventories = await _uow.Inventory.GetAllWithProductAsync();

        return inventories.Select(i => new InventoryReportDto
        {
            ProductId    = i.ProductId,
            ProductName  = i.Product.Name,
            SKU          = i.Product.SKU,
            CategoryName = i.Product.Category?.Name ?? "—",
            Unit         = i.Product.Unit,
            CurrentStock = i.CurrentStock,
            MinimumStock = i.Product.MinimumStock,
            CostPrice    = i.Product.CostPrice,
            SellingPrice = i.Product.SellingPrice,
            LastUpdated  = i.LastUpdated,
        }).OrderBy(r => r.StockStatus == "out" ? 0 : r.StockStatus == "low" ? 1 : 2)
          .ThenBy(r => r.CurrentStock);
    }

    // ─────────────────────────────────────────────────────────
    // 7.6  Profit & Loss Report
    // ─────────────────────────────────────────────────────────
    public async Task<ProfitLossReportDto> GetProfitLossReportAsync(ReportFilterDto filter)
    {
        var dateFrom = filter.DateFrom ?? DateTime.Today.AddDays(-29);
        var dateTo   = filter.DateTo   ?? DateTime.Today;

        var sales = await _uow.Sales.GetSalesForReportAsync(dateFrom, dateTo);

        var report = new ProfitLossReportDto
        {
            TotalSales   = sales.Count,
            TotalRevenue = sales.Sum(s => s.TotalAmount),
            TotalCost    = sales.Sum(s => s.SaleItems.Sum(si => si.UnitCost * si.Quantity)),
            GrossProfit  = sales.Sum(s => s.TotalProfit),
            TotalDiscount = sales.Sum(s => s.Discount),
        };

        // All sale items flattened
        var allItems = sales.SelectMany(s => s.SaleItems).ToList();

        // Group by Category
        report.CategoryRows = allItems
            .GroupBy(si => si.Product?.Category?.Name ?? "غير محدد")
            .Select(g => new ProfitLossRowDto
            {
                Name          = g.Key,
                TotalQuantity = g.Sum(si => si.Quantity),
                Revenue       = g.Sum(si => si.LineTotal),
                Cost          = g.Sum(si => si.UnitCost * si.Quantity),
                Profit        = g.Sum(si => si.LineProfit),
            })
            .OrderByDescending(r => r.Profit)
            .ToList();

        // Group by Product (top 20 by profit)
        report.ProductRows = allItems
            .GroupBy(si => new { si.ProductId, si.Product?.Name })
            .Select(g => new ProfitLossRowDto
            {
                Id            = g.Key.ProductId,
                Name          = g.Key.Name ?? "—",
                TotalQuantity = g.Sum(si => si.Quantity),
                Revenue       = g.Sum(si => si.LineTotal),
                Cost          = g.Sum(si => si.UnitCost * si.Quantity),
                Profit        = g.Sum(si => si.LineProfit),
            })
            .OrderByDescending(r => r.Profit)
            .Take(20)
            .ToList();

        return report;
    }

    // ─────────────────────────────────────────────────────────
    // 7.7  Low-Stock Report
    // ─────────────────────────────────────────────────────────
    public async Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync()
    {
        var items = await _uow.Inventory.GetLowStockItemsAsync();

        return items.Select(i => new LowStockReportDto
        {
            ProductId    = i.ProductId,
            ProductName  = i.Product.Name,
            SKU          = i.Product.SKU,
            CategoryName = i.Product.Category?.Name ?? "—",
            Unit         = i.Product.Unit,
            CurrentStock = i.CurrentStock,
            MinimumStock = i.Product.MinimumStock,
        }).OrderBy(r => r.CurrentStock);
    }
}
