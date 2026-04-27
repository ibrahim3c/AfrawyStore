using System.ComponentModel.DataAnnotations;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Application.DTOs;

/// <summary>
/// Read model for the sales list table
/// </summary>
public class SaleDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal Discount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public SaleStatus Status { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public string? Note { get; set; }

    public string PaymentMethodLabel => PaymentMethod switch
    {
        PaymentMethod.Cash => "نقدي",
        PaymentMethod.Card => "بطاقة",
        PaymentMethod.Other => "أخرى",
        _ => "غير محدد"
    };

    public string StatusLabel => Status switch
    {
        SaleStatus.Completed => "مكتملة",
        SaleStatus.Voided => "ملغاة",
        _ => "غير محدد"
    };
}

/// <summary>
/// Full sale detail with line items — used for receipt / detail view
/// </summary>
public class SaleDetailDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal Discount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public SaleStatus Status { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();

    public string PaymentMethodLabel => PaymentMethod switch
    {
        PaymentMethod.Cash => "نقدي",
        PaymentMethod.Card => "بطاقة",
        PaymentMethod.Other => "أخرى",
        _ => "غير محدد"
    };

    public string StatusLabel => Status switch
    {
        SaleStatus.Completed => "مكتملة",
        SaleStatus.Voided => "ملغاة",
        _ => "غير محدد"
    };
}

/// <summary>
/// Single line item in a sale
/// </summary>
public class SaleItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
    public decimal LineProfit { get; set; }
}

/// <summary>
/// Input model for creating a new sale from POS
/// </summary>
public class CreateSaleDto
{
    [Required(ErrorMessage = "يجب إضافة منتج واحد على الأقل")]
    [MinLength(1, ErrorMessage = "يجب إضافة منتج واحد على الأقل")]
    public List<CreateSaleItemDto> Items { get; set; } = new();

    [Range(0, 1000000, ErrorMessage = "الخصم يجب أن يكون صفر أو أكثر")]
    public decimal Discount { get; set; } = 0;

    [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [MaxLength(300, ErrorMessage = "الملاحظة يجب ألا تتجاوز 300 حرف")]
    public string? Note { get; set; }
}

/// <summary>
/// Single item in a new sale request
/// </summary>
public class CreateSaleItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "الكمية مطلوبة")]
    [Range(0.01, 1000000, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
    public decimal Quantity { get; set; }
}

/// <summary>
/// Lightweight product model for POS search results
/// </summary>
public class PosProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal SellingPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal CurrentStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}

/// <summary>
/// Filter / pagination model for sales list
/// </summary>
public class SalesFilterDto
{
    public string? SearchTerm { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public SaleStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
