using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using MassTransit;

namespace DavidStudio.Core.Essentials.CompleteSample.Entities;

public sealed class Product : Entity<ProductId>,
    ITimedEntity, ISoftDeletable,
    ISelfManageable<Product, ProductCreateDto, ProductUpdateDto>
{
    private Product() { }

    public string Name { get; private set; } = null!;

    public decimal Price { get; private set; }

    public int StockCount { get; private set; }

    public ManufacturerId ManufacturerId { get; private init; }
    public Manufacturer Manufacturer { get; init; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ModifiedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public static Product Create(ProductCreateDto model)
    {
        if (model.StockCount < 0)
            throw new ArgumentOutOfRangeException(nameof(model.StockCount), model.StockCount, "Stock count cannot be negative");

        return new Product
        {
            Id = new ProductId(NewId.NextGuid()),
            Name = model.Name,
            Price = model.Price,
            StockCount = model.StockCount,
            ManufacturerId = model.ManufacturerId
        };
    }

    public void Update(ProductUpdateDto model)
    {
        if (model.StockCount < 0)
            throw new ArgumentOutOfRangeException(nameof(model.StockCount), model.StockCount, "Stock count cannot be negative");

        Name = model.Name;
        Price = model.Price;
        StockCount = model.StockCount;
    }
}