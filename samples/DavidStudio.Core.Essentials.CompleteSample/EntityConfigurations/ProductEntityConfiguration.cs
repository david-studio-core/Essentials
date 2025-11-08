using DavidStudio.Core.Essentials.CompleteSample.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DavidStudio.Core.Essentials.CompleteSample.EntityConfigurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(e => e.Price).HasColumnType("decimal(10,2)");
    }
}