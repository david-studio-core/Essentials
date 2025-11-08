using DavidStudio.Core.DataIO.Repositories;
using DavidStudio.Core.Essentials.CompleteSample.Database;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Repositories;

public class ProductsRepository(ApplicationDbContext context)
    : BaseRepository<Product, ProductId>(context),
        IProductsRepository;