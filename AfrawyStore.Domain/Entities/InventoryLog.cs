using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Domain.Entities;

public class InventoryLog : BaseEntity
{
    public int ProductId { get; set; }
    public InventoryChangeType ChangeType { get; set; }
    public decimal QuantityChange { get; set; }
    public string? Note { get; set; }
    public int CreatedById { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}
