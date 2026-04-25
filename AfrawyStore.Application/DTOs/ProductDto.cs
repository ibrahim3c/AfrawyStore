using System.ComponentModel.DataAnnotations;

namespace AfrawyStore.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }
    public decimal CurrentStock { get; set; }
}

public class ProductCreateDto
{
    [Required(ErrorMessage = "رمز المنتج (SKU) مطلوب")]
    [MaxLength(50, ErrorMessage = "رمز المنتج يجب ألا يتجاوز 50 حرف")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [MaxLength(150, ErrorMessage = "اسم المنتج يجب ألا يتجاوز 150 حرف")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "الفئة مطلوبة")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "سعر التكلفة مطلوب")]
    [Range(0.01, 1000000, ErrorMessage = "سعر التكلفة يجب أن يكون أكبر من صفر")]
    public decimal CostPrice { get; set; }

    [Required(ErrorMessage = "سعر البيع مطلوب")]
    [Range(0.01, 1000000, ErrorMessage = "سعر البيع يجب أن يكون أكبر من صفر")]
    public decimal SellingPrice { get; set; }

    [Required(ErrorMessage = "وحدة القياس مطلوبة")]
    [MaxLength(20, ErrorMessage = "وحدة القياس يجب ألا تتجاوز 20 حرف")]
    public string Unit { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class ProductEditDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "رمز المنتج (SKU) مطلوب")]
    [MaxLength(50, ErrorMessage = "رمز المنتج يجب ألا يتجاوز 50 حرف")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [MaxLength(150, ErrorMessage = "اسم المنتج يجب ألا يتجاوز 150 حرف")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "الفئة مطلوبة")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "سعر التكلفة مطلوب")]
    [Range(0.01, 1000000, ErrorMessage = "سعر التكلفة يجب أن يكون أكبر من صفر")]
    public decimal CostPrice { get; set; }

    [Required(ErrorMessage = "سعر البيع مطلوب")]
    [Range(0.01, 1000000, ErrorMessage = "سعر البيع يجب أن يكون أكبر من صفر")]
    public decimal SellingPrice { get; set; }

    [Required(ErrorMessage = "وحدة القياس مطلوبة")]
    [MaxLength(20, ErrorMessage = "وحدة القياس يجب ألا تتجاوز 20 حرف")]
    public string Unit { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string? ExistingImagePath { get; set; }
}
