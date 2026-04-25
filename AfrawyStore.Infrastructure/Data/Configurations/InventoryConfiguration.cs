using AfrawyStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfrawyStore.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CurrentStock).HasColumnType("decimal(10,2)").HasDefaultValue(0);
        builder.Property(x => x.MinimumStock).HasColumnType("decimal(10,2)").HasDefaultValue(5);

        builder.HasOne(x => x.Product)
            .WithOne(x => x.Inventory)
            .HasForeignKey<Inventory>(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
