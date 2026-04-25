using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Interfaces;
using AfrawyStore.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AfrawyStore.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(string? searchTerm, int? categoryId, bool? isActive, int page, int pageSize)
    {
        var query = _context.Set<Product>()
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerTerm) || p.SKU.ToLower().Contains(lowerTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
    {
        var query = _context.Set<Product>().Where(p => p.SKU == sku);
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return !await query.AnyAsync();
    }
}
