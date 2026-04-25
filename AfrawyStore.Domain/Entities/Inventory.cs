namespace AfrawyStore.Domain.Entities;

public class Inventory : BaseEntity
{
    public int ProductId { get; set; }
    public decimal CurrentStock { get; set; } = 0;
    public decimal MinimumStock { get; set; } = 5;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation
    public Product Product { get; set; } = null!;
}
