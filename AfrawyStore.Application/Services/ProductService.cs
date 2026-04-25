using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            SKU = p.SKU,
            Name = p.Name,
            CategoryId = p.CategoryId,
            CostPrice = p.CostPrice,
            SellingPrice = p.SellingPrice,
            Unit = p.Unit,
            IsActive = p.IsActive
        }).ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var p = await _unitOfWork.Products.GetByIdAsync(id);
        if (p == null) return null;
        
        return new ProductDto
        {
            Id = p.Id,
            SKU = p.SKU,
            Name = p.Name,
            CategoryId = p.CategoryId,
            CostPrice = p.CostPrice,
            SellingPrice = p.SellingPrice,
            Unit = p.Unit,
            IsActive = p.IsActive,
            Description = p.Description,
            ImagePath = p.ImagePath
        };
    }

    public async Task CreateProductAsync(Product product)
    {
        // Enforce basic business logic: SellingPrice >= CostPrice
        if (product.SellingPrice < product.CostPrice)
            throw new InvalidOperationException("Selling price must be greater than or equal to cost price.");

        // Automatically create an inventory record for the new product
        product.Inventory = new Inventory
        {
            CurrentStock = 0,
            MinimumStock = 5,
            LastUpdated = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        if (product.SellingPrice < product.CostPrice)
            throw new InvalidOperationException("Selling price must be greater than or equal to cost price.");

        var existing = await _unitOfWork.Products.GetByIdAsync(product.Id);
        if (existing != null)
        {
            existing.SKU = product.SKU;
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.CategoryId = product.CategoryId;
            existing.CostPrice = product.CostPrice;
            existing.SellingPrice = product.SellingPrice;
            existing.Unit = product.Unit;
            existing.IsActive = product.IsActive;
            
            _unitOfWork.Products.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product != null)
        {
            // Soft delete by setting IsActive to false
            product.IsActive = false;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
