using DavidStudio.Core.Essentials.CompleteSample.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DavidStudio.Core.Essentials.CompleteSample.EntityConfigurations;

public class ManufacturerEntityConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(100);
    }
}