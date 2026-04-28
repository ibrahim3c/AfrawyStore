using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<ProductDto>> GetPagedProductsAsync(string? searchTerm, int? categoryId, bool? isActive, int page, int pageSize)
    {
        var (products, totalCount) = await _unitOfWork.Products.GetPagedProductsAsync(searchTerm, categoryId, isActive, page, pageSize);
        
        var items = products.Select(p => new ProductDto
        {
            Id = p.Id,
            SKU = p.SKU,
            Name = p.Name,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            CostPrice = p.CostPrice,
            SellingPrice = p.SellingPrice,
            Unit = p.Unit,
            IsActive = p.IsActive,
            ImagePath = p.ImagePath,
            CurrentStock = p.Inventory?.CurrentStock ?? 0,
            MinimumStock = p.MinimumStock
        }).ToList();

        return new PagedResultDto<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        // Need eager loading for Category and Inventory. We'll add this via a direct query since GetByIdAsync in generic repository doesn't Include by default, 
        // but for now we'll do a focused query if needed, or just return basic info if CategoryName isn't strictly needed here.
        // Actually, it's better to get the detailed info. Let's get the products from the paginated query as a workaround for now to get details if we just need one, or just use the generic and fetch category manually.
        // For simplicity, let's just use the generic GetById. It won't have CategoryName unless we load it.
        // A better approach is to use the generic but we'll add Include manually if we had access to IQueryable, but we don't.
        // So we'll just get the product and if we need CategoryName, we fetch it.
        var p = await _unitOfWork.Products.GetByIdAsync(id);
        if (p == null) return null;
        
        var category = await _unitOfWork.Categories.GetByIdAsync(p.CategoryId);
        var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(id);
        
        return new ProductDto
        {
            Id = p.Id,
            SKU = p.SKU,
            Name = p.Name,
            CategoryId = p.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            CostPrice = p.CostPrice,
            SellingPrice = p.SellingPrice,
            Unit = p.Unit,
            IsActive = p.IsActive,
            Description = p.Description,
            ImagePath = p.ImagePath,
            CurrentStock = inventory?.CurrentStock ?? 0,
            MinimumStock = p.MinimumStock
        };
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
    {
        return await _unitOfWork.Products.IsSkuUniqueAsync(sku, excludeId);
    }

    public async Task<bool> CreateProductAsync(ProductCreateDto createDto, string? imagePath, int userId)
    {
        if (createDto.SellingPrice < createDto.CostPrice)
            return false;

        var isSkuUnique = await IsSkuUniqueAsync(createDto.SKU);
        if (!isSkuUnique)
            return false;

        var product = new Product
        {
            SKU = createDto.SKU,
            Name = createDto.Name,
            Description = createDto.Description,
            CategoryId = createDto.CategoryId,
            CostPrice = createDto.CostPrice,
            SellingPrice = createDto.SellingPrice,
            Unit = createDto.Unit,
            MinimumStock = createDto.MinimumStock,
            IsActive = createDto.IsActive,
            ImagePath = imagePath,
            Inventory = new Inventory
            {
                CurrentStock = createDto.InitialStock,
                LastUpdated = DateTime.UtcNow
            }
        };

        if (createDto.InitialStock > 0)
        {
            product.InventoryLogs.Add(new InventoryLog
            {
                ChangeType = InventoryChangeType.StockIn,
                QuantityChange = createDto.InitialStock,
                Note = "الرصيد الافتتاحي عند إنشاء المنتج",
                CreatedAt = DateTime.UtcNow,
                CreatedById = userId
            });
        }

        await _unitOfWork.Products.AddAsync(product);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateProductAsync(ProductEditDto editDto, string? newImagePath)
    {
        if (editDto.SellingPrice < editDto.CostPrice)
            return false;

        var isSkuUnique = await IsSkuUniqueAsync(editDto.SKU, editDto.Id);
        if (!isSkuUnique)
            return false;

        var existing = await _unitOfWork.Products.GetByIdAsync(editDto.Id);
        if (existing == null)
            return false;

        existing.SKU = editDto.SKU;
        existing.Name = editDto.Name;
        existing.Description = editDto.Description;
        existing.CategoryId = editDto.CategoryId;
        existing.CostPrice = editDto.CostPrice;
        existing.SellingPrice = editDto.SellingPrice;
        existing.Unit = editDto.Unit;
        existing.MinimumStock = editDto.MinimumStock;
        existing.IsActive = editDto.IsActive;
        
        if (newImagePath != null)
        {
            existing.ImagePath = newImagePath;
        }

        _unitOfWork.Products.Update(existing);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return false;
        
        // Soft delete
        product.IsActive = false;
        _unitOfWork.Products.Update(product);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> BulkToggleStatusAsync(int[] productIds, bool isActive)
    {
        if (productIds == null || productIds.Length == 0) return false;

        foreach (var id in productIds)
        {
            var p = await _unitOfWork.Products.GetByIdAsync(id);
            if (p != null)
            {
                p.IsActive = isActive;
                _unitOfWork.Products.Update(p);
            }
        }

        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}
