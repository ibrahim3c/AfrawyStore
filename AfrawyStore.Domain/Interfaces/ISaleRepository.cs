using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Domain.Interfaces;

public interface ISaleRepository : IGenericRepository<Sale>
{
    Task<Sale?> GetSaleWithItemsAsync(int saleId);
    Task<(IEnumerable<Sale> Sales, int TotalCount)> GetPagedSalesAsync(
        string? searchTerm, DateTime? dateFrom, DateTime? dateTo,
        SaleStatus? status, int page, int pageSize);
    /// <summary>All completed sales on or after <paramref name="fromDate"/> (UTC).</summary>
    Task<List<Sale>> GetSalesSinceDateAsync(DateTime fromDate);
    /// <summary>Most recent <paramref name="count"/> sales (any status), with CreatedBy loaded.</summary>
    Task<List<Sale>> GetLatestSalesAsync(int count);

    /// <summary>
    /// Completed sales within the given date range, with SaleItems + Product + Category loaded.
    /// Used exclusively by the Reports module.
    /// </summary>
    Task<List<Sale>> GetSalesForReportAsync(DateTime? dateFrom, DateTime? dateTo);
}
