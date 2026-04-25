using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllWithParentAsync();
        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<bool> CreateCategoryAsync(CategoryCreateDto createDto)
    {
        var category = new Category
        {
            Name = createDto.Name,
            Description = createDto.Description,
            ParentCategoryId = createDto.ParentCategoryId
        };

        await _unitOfWork.Categories.AddAsync(category);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCategoryAsync(CategoryEditDto editDto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(editDto.Id);
        if (category == null) return false;

        // Prevent setting parent to itself
        if (editDto.ParentCategoryId == editDto.Id) return false;

        category.Name = editDto.Name;
        category.Description = editDto.Description;
        category.ParentCategoryId = editDto.ParentCategoryId;

        _unitOfWork.Categories.Update(category);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<string> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return "الفئة غير موجودة.";

        // Check for products
        var hasProducts = await _unitOfWork.Categories.HasProductsAsync(id);
        if (hasProducts)
            return "لا يمكن حذف هذه الفئة لأنها تحتوي على منتجات.";

        // Check for subcategories
        var hasSubCategories = await _unitOfWork.Categories.HasSubCategoriesAsync(id);
        if (hasSubCategories)
            return "لا يمكن حذف هذه الفئة لأنها تحتوي على فئات فرعية.";

        _unitOfWork.Categories.Delete(category);
        var success = await _unitOfWork.SaveChangesAsync() > 0;
        
        return success ? string.Empty : "حدث خطأ أثناء محاولة حذف الفئة.";
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name
        };
    }
}
