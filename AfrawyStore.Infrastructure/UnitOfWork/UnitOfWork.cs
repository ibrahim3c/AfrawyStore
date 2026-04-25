using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Domain.Interfaces;
using AfrawyStore.Infrastructure.Data;
using AfrawyStore.Infrastructure.Repositories;

namespace AfrawyStore.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IInventoryRepository Inventory { get; }
    public ISaleRepository Sales { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new ProductRepository(context);
        Categories = new CategoryRepository(context);
        Inventory = new InventoryRepository(context);
        Sales = new SaleRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
