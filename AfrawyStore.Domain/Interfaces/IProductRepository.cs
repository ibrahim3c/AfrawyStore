using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(string? searchTerm, int? categoryId, bool? isActive, int page, int pageSize);
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
}
