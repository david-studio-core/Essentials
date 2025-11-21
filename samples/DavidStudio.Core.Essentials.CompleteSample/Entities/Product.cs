using DavidStudio.Core.Auth.StronglyTypedIds;
using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.Essentials.CompleteSample.Models.Product;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using MassTransit;

namespace DavidStudio.Core.Essentials.CompleteSample.Entities;

public sealed class Product : Entity<ProductId>,
    ITimedEntity, ISoftDeletable,
    ISelfManageable<Product, ProductCreateModel, ProductUpdateModel>
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockCount { get; set; }

    public ManufacturerId ManufacturerId { get; init; }
    public Manufacturer Manufacturer { get; init; } = null!;

    public IdentityId CreatedByUserId { get; init; }
    public IdentityId? ModifiedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime ModifiedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public static Product Create(ProductCreateModel model)
    {
        if (model.StockCount < 0)
            throw new ArgumentOutOfRangeException(nameof(model.StockCount), model.StockCount, "Stock count cannot be negative");

        return new Product
        {
            Id = new ProductId(NewId.NextGuid()),
            Name = model.Name,
            Price = model.Price,
            StockCount = model.StockCount,
            ManufacturerId = model.ManufacturerId,
            CreatedByUserId = model.UserId,
            ModifiedByUserId = null
        };
    }

    public void Update(ProductUpdateModel model)
    {
        if (model.StockCount < 0)
            throw new ArgumentOutOfRangeException(nameof(model.StockCount), model.StockCount, "Stock count cannot be negative");

        Name = model.Name;
        Price = model.Price;
        StockCount = model.StockCount;
        ModifiedByUserId = model.UserId;
    }
}