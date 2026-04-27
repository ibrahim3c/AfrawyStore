using AfrawyStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfrawyStore.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SKU).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.SKU).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.CostPrice).HasColumnType("decimal(10,2)");
        builder.Property(x => x.SellingPrice).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Unit).HasMaxLength(30);
        builder.Property(x => x.ImagePath).HasMaxLength(300);
        builder.Property(x => x.MinimumStock).HasColumnType("decimal(10,2)").HasDefaultValue(0);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
