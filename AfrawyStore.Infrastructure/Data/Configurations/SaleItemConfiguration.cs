using AfrawyStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfrawyStore.Infrastructure.Data.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("decimal(10,2)");
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(10,2)");
        builder.Property(x => x.UnitCost).HasColumnType("decimal(10,2)");
        builder.Property(x => x.LineTotal).HasColumnType("decimal(10,2)");
        builder.Property(x => x.LineProfit).HasColumnType("decimal(10,2)");

        builder.HasOne(x => x.Sale)
            .WithMany(x => x.SaleItems)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.SaleItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
