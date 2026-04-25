using AfrawyStore.Application.DTOs;

namespace AfrawyStore.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<bool> CreateCategoryAsync(CategoryCreateDto createDto);
    Task<bool> UpdateCategoryAsync(CategoryEditDto editDto);
    
    /// <summary>
    /// Deletes a category. Returns an error message if it cannot be deleted (e.g. has products/subcategories), 
    /// or empty string if successful.
    /// </summary>
    Task<string> DeleteCategoryAsync(int id);
}
