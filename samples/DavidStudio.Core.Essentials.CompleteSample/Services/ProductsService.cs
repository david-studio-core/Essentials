using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Services;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.Mappers;
using DavidStudio.Core.Essentials.CompleteSample.Repositories;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Services;

public class ProductsService(IProductsRepository repository)
    : BaseReadonlyService<IProductsRepository, Product, ProductId, ProductReadDto>(repository),
        IProductsService
{
    protected override Expression<Func<Product, ProductReadDto>> ToReadDto => e => e.ToReadDto();
}