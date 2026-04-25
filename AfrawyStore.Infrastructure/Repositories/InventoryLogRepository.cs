using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Interfaces;
using AfrawyStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AfrawyStore.Infrastructure.Repositories;

public class InventoryLogRepository : GenericRepository<InventoryLog>, IInventoryLogRepository
{
    public InventoryLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InventoryLog>> GetLogsByProductIdAsync(int productId)
    {
        return await _context.Set<InventoryLog>()
            .Include(l => l.CreatedBy)
            .Where(l => l.ProductId == productId)
            .OrderByDescending(l => l.Id)
            .ToListAsync();
    }
}
