using DavidStudio.Core.DataIO.Services;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Services;

public interface IProductsService : IBaseReadonlyService<Product, ProductId, ProductReadDto>;