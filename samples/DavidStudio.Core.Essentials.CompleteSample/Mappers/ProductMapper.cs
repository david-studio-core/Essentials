using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.Entities;

namespace DavidStudio.Core.Essentials.CompleteSample.Mappers;

public static class ProductMapper
{
    public static ProductReadDto ToReadDto(this Product product)
    {
        return new ProductReadDto(
            product.Id,
            product.Name,
            product.Price,
            product.StockCount,
            product.Manufacturer.ToReadDto(),
            product.CreatedByUserId,
            product.ModifiedByUserId,
            product.CreatedAtUtc,
            product.ModifiedAtUtc
        );
    }
}