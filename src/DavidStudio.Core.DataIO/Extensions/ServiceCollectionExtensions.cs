using System.Reflection;
using DavidStudio.Core.DataIO.Interceptors;
using DavidStudio.Core.DataIO.UnitOfWork.ADO.NET;
using DavidStudio.Core.DataIO.UnitOfWork.EfCore;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace DavidStudio.Core.DataIO.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services, string? connectionString,
        string? assemblyName = null)
        where TDbContext : DbContext
    {
        assemblyName ??= Assembly.GetExecutingAssembly().GetName().Name;

        services.AddDbContext<TDbContext>((sp, options) =>
        {
            connectionString ??= sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

            options
                .UseSqlServer(connectionString, x => x.MigrationsAssembly(assemblyName))
                .AddInterceptors(
                    new TimedEntitiesInterceptor(),
                    new SoftDeleteInterceptor());
        });

        return services;
    }

    public static IServiceCollection AddEfUnitOfWork<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        return services.AddScoped<IEfUnitOfWork<TDbContext>>(sp
            => new EfUnitOfWork<TDbContext>(sp.GetRequiredService<TDbContext>()));
    }

    public static IServiceCollection AddAdoUnitOfWork(this IServiceCollection services, string? connectionString)
    {
        return services.AddScoped<IAdoUnitOfWork>(sp =>
        {
            connectionString ??= sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")
                                 ?? throw new InvalidOperationException("No sql connection string found.");

            return new AdoUnitOfWork(connectionString);
        });
    }

    public static IServiceProvider MigrateDatabase<TDbContext>(this IServiceProvider services)
        where TDbContext : DbContext
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        db.Database.Migrate();

        return services;
    }

    public static void AddElasticsearchClient(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Elasticsearch")
                               ?? throw new ArgumentException("Elasticsearch connection string is missing in the configuration.");

        services.AddSingleton<ElasticsearchClient>(_ =>
        {
            var connectionPool = new SingleNodePool(new Uri(connectionString));
            var connectionSettings = new ElasticsearchClientSettings(connectionPool)
                .RequestTimeout(TimeSpan.FromSeconds(10))
                .MaximumRetries(5);

            return new ElasticsearchClient(connectionSettings);
        });
    }

    public static IServiceCollection AddRedis(this IServiceCollection services,
        string? connectionString)
    {
        return services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            connectionString ??= sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")
                                 ?? throw new InvalidOperationException("No redis connection string found.");

            var redis = ConnectionMultiplexer.Connect(connectionString);

            return redis;
        });
    }

    public static IServiceCollection AddRedLock(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = configuration.GetSection("RedLock:Endpoints").Get<List<string>>()
                                ?? throw new InvalidOperationException("RedLock endpoints are not configured.");

        var multiplexers = connectionStrings
            .Select(connectionString => (RedLockMultiplexer)ConnectionMultiplexer.Connect(connectionString))
            .ToList();

        var redLockFactory = RedLockFactory.Create(multiplexers);

        return services.AddSingleton<IDistributedLockFactory>(redLockFactory);
    }
}