using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;

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

        var apple = Manufacturer.Create(new ManufacturerCreateDto
        {
            Name = "Apple",
            IncorporationDateUtc = new DateTime(1976, 1, 1)
        });

        var samsung = Manufacturer.Create(new ManufacturerCreateDto
        {
            Name = "Samsung",
            IncorporationDateUtc = new DateTime(1938, 1, 1)
        });

        var xiaomi = Manufacturer.Create(new ManufacturerCreateDto
        {
            Name = "Xiaomi",
            IncorporationDateUtc = new DateTime(2010, 1, 1)
        });

        var iPhone17 = Product.Create(new ProductCreateDto
        {
            Name = "iPhone 17",
            Price = 1199,
            StockCount = 1000,
            ManufacturerId = apple.Id
        });

        var iPhone16 = Product.Create(new ProductCreateDto
        {
            Name = "iPhone 16",
            Price = 999,
            StockCount = 50,
            ManufacturerId = apple.Id
        });

        var samsungGalaxyS25 = Product.Create(new ProductCreateDto
        {
            Name = "Samsung Galaxy S25",
            Price = 25,
            StockCount = 1_000_000,
            ManufacturerId = samsung.Id
        });

        var xiaomi17ProMax = Product.Create(new ProductCreateDto
        {
            Name = "Xiaomi 17 Pro Max",
            Price = 1500,
            StockCount = 10,
            ManufacturerId = xiaomi.Id
        });

        builder.Entity<Manufacturer>().HasData(apple, samsung, xiaomi);
        builder.Entity<Product>().HasData(iPhone17, iPhone16, samsungGalaxyS25, xiaomi17ProMax);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<ProductId>().HaveConversion<ProductId.EfCoreValueConverter>();
        configurationBuilder.Properties<ManufacturerId>().HaveConversion<ManufacturerId.EfCoreValueConverter>();
    }
}