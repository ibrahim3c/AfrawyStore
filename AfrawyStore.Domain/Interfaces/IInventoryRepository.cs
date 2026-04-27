using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Domain.Interfaces;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    Task<IEnumerable<Inventory>> GetAllWithProductAsync();
    Task<Inventory?> GetByProductIdAsync(int productId);
    Task<int> GetLowStockCountAsync();
    /// <summary>All active products where CurrentStock ≤ MinimumStock, ordered by stock ascending.</summary>
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync();
}
