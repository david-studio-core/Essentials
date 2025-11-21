using DavidStudio.Core.Auth.StronglyTypedIds;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Models.Product;

public record ProductCreateModel(
    string Name,
    decimal Price,
    int StockCount,
    ManufacturerId ManufacturerId,
    IdentityId UserId
);