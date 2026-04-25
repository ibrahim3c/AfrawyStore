using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Interfaces;
using AfrawyStore.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AfrawyStore.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllWithParentAsync()
    {
        return await _context.Set<Category>()
            .Include(c => c.ParentCategory)
            .ToListAsync();
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Set<Product>()
            .AnyAsync(p => p.CategoryId == categoryId);
    }

    public async Task<bool> HasSubCategoriesAsync(int categoryId)
    {
        return await _context.Set<Category>()
            .AnyAsync(c => c.ParentCategoryId == categoryId);
    }
}
