using System.Reflection;
using System.Security.Authentication;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DavidStudio.Core.DataIO.Extensions;

/// <summary>
/// Provides extension methods for registering and configuring MassTransit-based event bus transports.
/// </summary>
public static class EventBusExtensions
{
    /// <summary>
    /// Registers and configures MassTransit with RabbitMQ transport.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> used to register services.</param>
    /// <param name="configSectionKey">
    /// The configuration section key for binding <see cref="RabbitMqTransportOptions"/>.
    /// If not specified, the default section name <c>RabbitMqTransportOptions</c> is used.
    /// </param>
    /// <param name="assembly">
    /// The assembly containing message consumers.
    /// If not specified, the executing assembly is used.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method configures MassTransit with RabbitMQ, applies exponential retry policies,
    /// and enables SSL if specified in configuration.
    /// </remarks>
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, string? configSectionKey = null, Assembly? assembly = null)
    {
        services.AddOptions<RabbitMqTransportOptions>()
            .BindConfiguration(configSectionKey ?? nameof(RabbitMqTransportOptions));

        assembly ??= Assembly.GetExecutingAssembly();

        services.AddMassTransit(busConfiguration =>
        {
            busConfiguration.SetKebabCaseEndpointNameFormatter();

            busConfiguration.AddConsumers(assembly);

            busConfiguration.UsingRabbitMq((context, config) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqTransportOptions>>().Value;

                config.Host(options.Host, options.Port, options.VHost, h =>
                {
                    h.Username(options.User);
                    h.Password(options.Pass);

                    if (options.UseSsl)
                        h.UseSsl(s => { s.Protocol = SslProtocols.Tls12; });
                });

                config.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Exponential(
                        retryLimit: 5,
                        minInterval: TimeSpan.FromMilliseconds(100),
                        maxInterval: TimeSpan.FromSeconds(30),
                        intervalDelta: TimeSpan.FromSeconds(5)
                    );
                });

                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Registers and configures MassTransit with Azure Service Bus transport.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> used to register services.</param>
    /// <param name="configSectionKey">
    /// The configuration section key for binding <see cref="AzureServiceBusTransportOptions"/>.
    /// If not specified, the default section name <c>AzureServiceBusTransportOptions</c> is used.
    /// </param>
    /// <param name="assembly">
    /// The assembly containing message consumers.
    /// If not specified, the executing assembly is used.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method configures MassTransit with Azure Service Bus using the provided connection string.
    /// </remarks>
    public static IServiceCollection AddAzureServiceBus(this IServiceCollection services, string? configSectionKey = null, Assembly? assembly = null)
    {
        services.AddOptions<AzureServiceBusTransportOptions>()
            .BindConfiguration(configSectionKey ?? nameof(AzureServiceBusTransportOptions));

        assembly ??= Assembly.GetExecutingAssembly();

        services.AddMassTransit(busConfiguration =>
        {
            busConfiguration.SetKebabCaseEndpointNameFormatter();

            busConfiguration.AddConsumers(assembly);

            busConfiguration.UsingAzureServiceBus((context, config) =>
            {
                var options = context.GetRequiredService<IOptions<AzureServiceBusTransportOptions>>().Value;

                config.Host(options.ConnectionString);

                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Registers and configures MassTransit with an in-memory transport for local testing or development.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> used to register services.</param>
    /// <param name="assembly">
    /// The assembly containing message consumers.
    /// If not specified, the executing assembly is used.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method configures MassTransit to use an in-memory transport suitable for unit tests or simple local setups.
    /// </remarks>
    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();

        services.AddMassTransit(busConfiguration =>
        {
            busConfiguration.SetKebabCaseEndpointNameFormatter();

            busConfiguration.AddConsumers(assembly);

            busConfiguration.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
        });

        return services;
    }
}