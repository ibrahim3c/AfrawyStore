using AfrawyStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfrawyStore.Infrastructure.Data.Configurations;

public class InventoryLogConfiguration : IEntityTypeConfiguration<InventoryLog>
{
    public void Configure(EntityTypeBuilder<InventoryLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChangeType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.QuantityChange).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Note).HasMaxLength(300);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.InventoryLogs)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.InventoryLogs)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
