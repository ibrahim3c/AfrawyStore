namespace AfrawyStore.Application.Interfaces.Services;

using AfrawyStore.Application.DTOs;
using AfrawyStore.Domain.Entities;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
