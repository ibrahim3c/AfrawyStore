using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Domain.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithParentAsync();
    Task<bool> HasProductsAsync(int categoryId);
    Task<bool> HasSubCategoriesAsync(int categoryId);
}
