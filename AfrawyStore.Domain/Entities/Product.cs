namespace AfrawyStore.Domain.Entities;

public class Product : BaseEntity
{
    //Stock Keeping Unit => barcode
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public decimal MinimumStock { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Category Category { get; set; } = null!;
    public Inventory Inventory { get; set; } = null!;
    public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
// profit = selling - cost
// Profit Margin = (profit / selling) * 100;
    // if for ex => 30% means 30% of sellingPrice is profit
