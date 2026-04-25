using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Domain.Entities;

public class Sale : BaseEntity
{
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal Discount { get; set; } = 0;
    public PaymentMethod PaymentMethod { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public int CreatedById { get; set; }
    public string? Note { get; set; }

    // Navigation
    public User CreatedBy { get; set; } = null!;
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
