using AfrawyStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfrawyStore.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(10,2)");
        builder.Property(x => x.TotalProfit).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Discount).HasColumnType("decimal(10,2)").HasDefaultValue(0);
        builder.Property(x => x.PaymentMethod).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Note).HasMaxLength(300);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.Sales)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => x.SaleDate);
    }
}
