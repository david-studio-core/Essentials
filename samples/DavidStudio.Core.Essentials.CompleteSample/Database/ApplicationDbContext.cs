using DavidStudio.Core.Auth.StronglyTypedIds;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.Models.Product;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DavidStudio.Core.Essentials.CompleteSample.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products { get; init; }
    public DbSet<Manufacturer> Manufacturers { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        var apple = new Manufacturer
        {
            Id = ManufacturerId.Parse("E9A85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "Apple",
            IncorporationDateUtc = new DateTime(1976, 1, 1)
        };

        var samsung = new Manufacturer
        {
            Id = ManufacturerId.Parse("EAA85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "Samsung",
            IncorporationDateUtc = new DateTime(1938, 1, 1)
        };

        var xiaomi = new Manufacturer
        {
            Id = ManufacturerId.Parse("EBA85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "Xiaomi",
            IncorporationDateUtc = new DateTime(2010, 1, 1)
        };

        var utcNow = new DateTime(2025, 11, 21);

        var iPhone17 = new Product
        {
            Id = ProductId.Parse("ECA85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "iPhone 17",
            Price = 1199,
            StockCount = 1000,
            ManufacturerId = apple.Id,
            CreatedByUserId = IdentityId.Empty,
            ModifiedByUserId = null,
            CreatedAtUtc = utcNow,
            ModifiedAtUtc = utcNow,
            IsDeleted = false
        };

        var iPhone16 = new Product
        {
            Id = ProductId.Parse("EDA85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "iPhone 16",
            Price = 999,
            StockCount = 50,
            ManufacturerId = apple.Id,
            CreatedByUserId = IdentityId.Empty,
            ModifiedByUserId = null,
            CreatedAtUtc = utcNow,
            ModifiedAtUtc = utcNow,
            IsDeleted = false
        };

        var samsungGalaxyS25 = new Product
        {
            Id = ProductId.Parse("EEA85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "Samsung Galaxy S25",
            Price = 600,
            StockCount = 25,
            ManufacturerId = samsung.Id,
            CreatedByUserId = IdentityId.Empty,
            ModifiedByUserId = null,
            CreatedAtUtc = utcNow,
            ModifiedAtUtc = utcNow,
            IsDeleted = false
        };

        var xiaomi17ProMax = new Product
        {
            Id = ProductId.Parse("EFA85B6D-B003-46AF-AD15-69B1A6CBDD7C"),
            Name = "Xiaomi 17 Pro Max",
            Price = 1500,
            StockCount = 10,
            ManufacturerId = xiaomi.Id,
            CreatedByUserId = IdentityId.Empty,
            ModifiedByUserId = null,
            CreatedAtUtc = utcNow,
            ModifiedAtUtc = utcNow,
            IsDeleted = false
        };

        builder.Entity<Manufacturer>().HasData(apple, samsung, xiaomi);
        builder.Entity<Product>().HasData(iPhone17, iPhone16, samsungGalaxyS25, xiaomi17ProMax);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<IdentityId>().HaveConversion<IdentityId.EfCoreValueConverter>();

        configurationBuilder.Properties<ProductId>().HaveConversion<ProductId.EfCoreValueConverter>();
        configurationBuilder.Properties<ManufacturerId>().HaveConversion<ManufacturerId.EfCoreValueConverter>();
    }
}