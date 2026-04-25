using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Domain.Interfaces;

public interface IInventoryLogRepository : IGenericRepository<InventoryLog>
{
    Task<IEnumerable<InventoryLog>> GetLogsByProductIdAsync(int productId);
}
