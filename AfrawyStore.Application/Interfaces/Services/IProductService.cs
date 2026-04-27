namespace AfrawyStore.Application.Interfaces.Services;

using AfrawyStore.Application.DTOs;
using AfrawyStore.Domain.Entities;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetPagedProductsAsync(string? searchTerm, int? categoryId, bool? isActive, int page, int pageSize);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
    Task<bool> CreateProductAsync(ProductCreateDto createDto, string? imagePath, int userId);
    Task<bool> UpdateProductAsync(ProductEditDto editDto, string? newImagePath);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> BulkToggleStatusAsync(int[] productIds, bool isActive);
}
