using AfrawyStore.Application.DTOs;

namespace AfrawyStore.Application.Interfaces.Services;

public interface ISaleService
{
    Task<IEnumerable<PosProductDto>> SearchProductsForPosAsync(string? term);
    Task<(int? SaleId, string? Error)> CreateSaleAsync(CreateSaleDto dto, int userId);
    Task<string?> VoidSaleAsync(int saleId, int userId);
    Task<SaleDetailDto?> GetSaleDetailAsync(int saleId);
    Task<(IEnumerable<SaleDto> Sales, int TotalCount)> GetPagedSalesAsync(SalesFilterDto filter);
}
