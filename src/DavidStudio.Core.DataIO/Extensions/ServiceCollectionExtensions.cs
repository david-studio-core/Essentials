using System.Reflection;
using DavidStudio.Core.DataIO.Interceptors;
using DavidStudio.Core.DataIO.UnitOfWork.ADO.NET;
using DavidStudio.Core.DataIO.UnitOfWork.EfCore;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace DavidStudio.Core.DataIO.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures an Entity Framework Core <typeparamref name="TDbContext"/> to the service collection.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the EF Core <see cref="DbContext"/>.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="connectionString">Optional database connection string. If null, will use "DefaultConnection" from configuration.</param>
    /// <param name="assemblyName">Optional assembly name for EF Core migrations. Defaults to the executing assembly name.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
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

    /// <summary>
    /// Registers an Entity Framework Core–based unit of work (<see cref="IEfUnitOfWork{TDbContext}"/>) in the DI container.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the EF Core <see cref="DbContext"/> used by the unit of work.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEfUnitOfWork<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        return services.AddScoped<IEfUnitOfWork<TDbContext>>(sp
            => new EfUnitOfWork<TDbContext>(sp.GetRequiredService<TDbContext>()));
    }

    /// <summary>
    /// Registers an ADO.NET–based unit of work (<see cref="IAdoUnitOfWork"/>) in the DI container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="connectionString">The database connection string. If null, will use "DefaultConnection" from configuration.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the connection string cannot be resolved.</exception>
    public static IServiceCollection AddAdoUnitOfWork(this IServiceCollection services, string? connectionString)
    {
        return services.AddScoped<IAdoUnitOfWork>(sp =>
        {
            connectionString ??= sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")
                                 ?? throw new InvalidOperationException("No sql connection string found.");

            return new AdoUnitOfWork(connectionString);
        });
    }

    /// <summary>
    /// Registers an Elasticsearch client (<see cref="ElasticsearchClient"/>) in the DI container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="connectionString">Optional Elasticsearch connection string. If null, will use "Elasticsearch" from ConnectionStings section.</param>
    /// <exception cref="ArgumentException">Thrown if the Elasticsearch connection string is missing in configuration.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the Elasticsearch connection string cannot be resolved.</exception>
    public static void AddElasticsearchClient(this IServiceCollection services, string? connectionString = null)
    {
        services.AddSingleton<ElasticsearchClient>(sp =>
        {
            connectionString ??= sp.GetRequiredService<IConfiguration>().GetConnectionString("Elasticsearch")
                                 ?? throw new InvalidOperationException("No elasticsearch connection string found.");

            var connectionPool = new SingleNodePool(new Uri(connectionString));
            var connectionSettings = new ElasticsearchClientSettings(connectionPool)
                .RequestTimeout(TimeSpan.FromSeconds(10))
                .MaximumRetries(5);

            return new ElasticsearchClient(connectionSettings);
        });
    }

    /// <summary>
    /// Registers a Redis connection (<see cref="IConnectionMultiplexer"/>) in the DI container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="connectionString">Optional Redis connection string. If null, will use "Redis" from ConnectionStings section.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Redis connection string cannot be resolved.</exception>
    public static IServiceCollection AddRedis(this IServiceCollection services, string? connectionString = null)
    {
        return services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            connectionString ??= sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")
                                 ?? throw new InvalidOperationException("No redis connection string found.");

            var redis = ConnectionMultiplexer.Connect(connectionString);

            return redis;
        });
    }

    /// <summary>
    /// Registers a distributed cache implementation based on the current host environment.
    /// In development, an in-memory cache is used, while in non-development environments
    /// a Redis-backed distributed cache is configured.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="environment">The current host environment used to determine the cache implementation.</param>
    /// <param name="configuration">The application configuration used to resolve the Redis connection string.</param>
    /// <param name="connectionString">Optional Redis connection string. If null, will use "Redis" from ConnectionStings section.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Redis connection string cannot be resolved.</exception>
    public static IServiceCollection AddDistributedCache(this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration,
        string? connectionString = null)
    {
        if (environment.IsDevelopment())
            services.AddDistributedMemoryCache();
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                connectionString ??= configuration.GetConnectionString("Redis")
                                     ?? throw new InvalidOperationException("No redis connection string found.");

                options.Configuration = connectionString;
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = true,
                    EndPoints = { connectionString }
                };
            });
        }

        return services;
    }

    /// <summary>
    /// Registers a RedLock distributed lock factory (<see cref="IDistributedLockFactory"/>) in the DI container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration used to obtain Redis endpoints for RedLock.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no RedLock endpoints are configured.</exception>
    /// <example>
    /// The following example demonstrates how to register RedLock with multiple Redis endpoints in an ASP.NET Core application:
    /// <code>
    /// // appsettings.json:
    /// {
    ///   "RedLock": {
    ///     "Endpoints": [
    ///       "redis1.example.com:6379",
    ///       "redis2.example.com:6379",
    ///       "redis3.example.com:6379"
    ///     ]
    ///   }
    /// }
    /// </code>
    /// </example>
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