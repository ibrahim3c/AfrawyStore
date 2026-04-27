using System.ComponentModel.DataAnnotations;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Application.DTOs;

public class InventoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// "ok", "low", "out" based on stock levels
    /// </summary>
    public string StockStatus => CurrentStock <= 0 ? "out" : CurrentStock <= MinimumStock ? "low" : "ok";
}

public class InventoryAdjustDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public string Unit { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع العملية مطلوب")]
    public InventoryChangeType ChangeType { get; set; }

    [Required(ErrorMessage = "الكمية مطلوبة")]
    [Range(0.01, 1000000, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
    public decimal Quantity { get; set; }

    [MaxLength(500, ErrorMessage = "الملاحظة يجب ألا تتجاوز 500 حرف")]
    public string? Note { get; set; }

    public decimal MinimumStock { get; set; }
}

public class InventoryLogDto
{
    public int Id { get; set; }
    public InventoryChangeType ChangeType { get; set; }
    public decimal QuantityChange { get; set; }
    public string? Note { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public string ChangeTypeLabel => ChangeType switch
    {
        InventoryChangeType.StockIn => "إضافة مخزون",
        InventoryChangeType.Adjustment => "تعديل / تصحيح",
        InventoryChangeType.Sale => "بيع",
        _ => "غير محدد"
    };
}
