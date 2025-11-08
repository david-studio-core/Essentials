using DavidStudio.Core.Essentials.CompleteSample.Repositories;
using DavidStudio.Core.Essentials.CompleteSample.Services;

namespace DavidStudio.Core.Essentials.CompleteSample.Extensions;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IManufacturersRepository, ManufacturersRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IManufacturersService, ManufacturersService>();

        return services;
    }
}