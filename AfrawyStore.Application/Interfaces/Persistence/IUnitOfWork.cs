using AfrawyStore.Domain.Interfaces;

namespace AfrawyStore.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IInventoryRepository Inventory { get; }
    IInventoryLogRepository InventoryLogs { get; }
    ISaleRepository Sales { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
}
