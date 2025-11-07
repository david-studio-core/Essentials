using System.Reflection;
using DavidStudio.Core.Utilities.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace DavidStudio.Core.Utilities.Extensions;

/// <summary>
/// Provides extension methods for configuring common services and libraries in an ASP.NET Core application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the host to use Serilog for logging based on the application's configuration.
    /// </summary>
    /// <param name="host">The <see cref="IHostBuilder"/> to configure.</param>
    /// <returns>The <see cref="IHostBuilder"/> instance for chaining.</returns>
    /// <remarks>
    /// Reads Serilog settings from the application's configuration (e.g., appsettings.json)
    /// and configures the logger accordingly.
    /// </remarks>
    public static IHostBuilder UseSerilogFromConfiguration(this IHostBuilder host)
    {
        host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));

        return host;
    }

    /// <summary>
    /// Registers default OpenTelemetry tracing and metrics services for ASP.NET Core applications.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add OpenTelemetry services to.</param>
    /// <param name="serviceName">Optional service name to identify telemetry. Defaults to the executing assembly name.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method configures:
    /// <list type="bullet">
    /// <item>Resource information using <see cref="serviceName"/>.</item>
    /// <item>Tracing for ASP.NET Core, HTTP client, Entity Framework Core, and gRPC clients.</item>
    /// <item>Metrics for ASP.NET Core and HTTP client.</item>
    /// <item>OTLP exporters for both tracing and metrics.</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddDefaultOpenTelemetry(this IServiceCollection services, string? serviceName = null)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName ?? Assembly.GetExecutingAssembly().GetName().Name!))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddEntityFrameworkCoreInstrumentation();
                tracing.AddGrpcClientInstrumentation();

                tracing.AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();

                metrics.AddOtlpExporter();
            });

        return services;
    }

    /// <summary>
    /// Configures default API versioning and versioned API explorer for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add API versioning services to.</param>
    /// <remarks>
    /// Sets up:
    /// <list type="bullet">
    /// <item>Default API version 1.0.</item>
    /// <item>Assumes the default version when unspecified by the client.</item>
    /// <item>Reports supported API versions in responses.</item>
    /// <item>Reads API version from URL segments, query string, headers, or media type.</item>
    /// <item>Configures the versioned API explorer to generate documentation groups with format "v{major}.{minor}".</item>
    /// </list>
    /// </remarks>
    public static void AddDefaultApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
    }

    /// <summary>
    /// Configures Cross-Origin Resource Sharing (CORS) based on application configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add CORS services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance containing CORS settings.</param>
    /// <remarks>
    /// Reads <see cref="ApplicationCorsOptions"/> from configuration and sets up a default CORS policy:
    /// <list type="bullet">
    /// <item>Allows any header or only those specified in AllowedHeaders.</item>
    /// <item>Allows any method or only those specified in AllowedMethods.</item>
    /// <item>Restricts origins if AllowedOrigins are specified.</item>
    /// <item>Always allows credentials.</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the CORS configuration section is missing or invalid.</exception>
    public static void AddCorsFromConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(nameof(ApplicationCorsOptions)).Get<ApplicationCorsOptions>()
            ?? throw new InvalidOperationException("Cors options not found in configuration.");

        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
            {
                if (corsOptions.AllowedHeaders is null)
                    policy.AllowAnyHeader();
                else
                    policy.WithHeaders(corsOptions.AllowedHeaders.Split(';'));

                if (corsOptions.AllowedMethods is null)
                    policy.AllowAnyMethod();
                else
                    policy.WithMethods(corsOptions.AllowedMethods.Split(';'));

                if (corsOptions.AllowedOrigins is not null)
                    policy.WithOrigins(corsOptions.AllowedOrigins.Split(';'));

                policy.AllowCredentials();
            })
        );
    }
}