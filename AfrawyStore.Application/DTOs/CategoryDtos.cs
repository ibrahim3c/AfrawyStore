using System.ComponentModel.DataAnnotations;

namespace AfrawyStore.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
}

public class CategoryCreateDto
{
    [Required(ErrorMessage = "اسم الفئة مطلوب")]
    [MaxLength(100, ErrorMessage = "اسم الفئة يجب ألا يتجاوز 100 حرف")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف")]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }
}

public class CategoryEditDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الفئة مطلوب")]
    [MaxLength(100, ErrorMessage = "اسم الفئة يجب ألا يتجاوز 100 حرف")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف")]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }
}
