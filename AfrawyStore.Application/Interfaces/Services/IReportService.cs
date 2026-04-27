using AfrawyStore.Application.DTOs;

namespace AfrawyStore.Application.Interfaces.Services;

public interface IReportService
{
    /// <summary>
    /// 7.4 — Aggregated sales data grouped by day for a date range.
    /// </summary>
    Task<SalesReportDto> GetSalesReportAsync(ReportFilterDto filter);

    /// <summary>
    /// 7.5 — Full inventory snapshot for all products.
    /// </summary>
    Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync();

    /// <summary>
    /// 7.6 — Profit and loss grouped by category and product.
    /// </summary>
    Task<ProfitLossReportDto> GetProfitLossReportAsync(ReportFilterDto filter);

    /// <summary>
    /// 7.7 — All items whose CurrentStock is at or below MinimumStock.
    /// </summary>
    Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync();
}
