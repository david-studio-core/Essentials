using DavidStudio.Core.Auth.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Models.Product;

public record ProductUpdateModel(
    string Name,
    decimal Price,
    int StockCount,
    IdentityId UserId
);