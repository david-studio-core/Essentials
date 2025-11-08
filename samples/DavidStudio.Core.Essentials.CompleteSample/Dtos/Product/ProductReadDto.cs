using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;

public record ProductReadDto(
    ProductId Id,
    string Name,
    decimal Price,
    int StockCount,
    ManufacturerReadDto Manufacturer,
    DateTime CreatedAtUtc,
    DateTime ModifiedAtUtc
);