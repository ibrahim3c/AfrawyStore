using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Interfaces;
using AfrawyStore.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AfrawyStore.Infrastructure.Repositories;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Inventory>> GetAllWithProductAsync()
    {
        return await _context.Set<Inventory>()
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Where(i => i.Product.IsActive)
            .OrderBy(i => i.CurrentStock)
            .ToListAsync();
    }

    public async Task<Inventory?> GetByProductIdAsync(int productId)
    {
        return await _context.Set<Inventory>()
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<int> GetLowStockCountAsync()
    {
        return await _context.Set<Inventory>()
            .Where(i => i.Product.IsActive)
            .CountAsync(i => i.CurrentStock <= i.MinimumStock);
    }
}
