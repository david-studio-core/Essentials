using System.Net;
using System.Reflection;
using Asp.Versioning;
using DavidStudio.Core.Auth.Conventions;
using DavidStudio.Core.Utilities.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DavidStudio.Core.Utilities.Extensions;

/// <summary>
/// Provides extension methods for configuring common services and libraries in an ASP.NET Core application.
/// </summary>
public static class ServiceCollectionExtensions
{
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
    public static void AddCorsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
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

    /// <summary>
    /// Configures <see cref="ForwardedHeadersOptions"/> from application configuration,
    /// including trusted proxy IP addresses and networks.  
    /// 
    /// The method binds the base options from configuration, then explicitly clears and
    /// repopulates <c>KnownProxies</c> and <c>KnownNetworks</c>/<c>KnownIPNetworks</c>
    /// to ensure only explicitly configured proxies are trusted.  
    /// 
    /// Supports both legacy and newer .NET versions via conditional compilation.
    /// </summary>
    public static IServiceCollection ConfigureForwardedHeadersOptionsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            configuration.GetSection(nameof(ForwardedHeadersOptions)).Bind(options);


            var knownProxiesSection =
                configuration.GetSection($"{nameof(ForwardedHeadersOptions)}:{nameof(ForwardedHeadersOptions.KnownProxies)}");

            options.KnownProxies.Clear();

            foreach (var proxyAddress in knownProxiesSection.GetChildren())
            {
                if (IPAddress.TryParse(proxyAddress.Value, out var ipAddress))
                {
                    options.KnownProxies.Add(ipAddress);
                }
            }

#if NET10_0_OR_GREATER
            var knownIpNetworksSection =
                configuration.GetSection($"{nameof(ForwardedHeadersOptions)}:{nameof(ForwardedHeadersOptions.KnownIPNetworks)}");

            options.KnownIPNetworks.Clear();

            foreach (var networkConfig in knownIpNetworksSection.GetChildren())
            {
                var prefix = networkConfig.GetValue<string>("Prefix");
                var prefixLength = networkConfig.GetValue<int>("PrefixLength");

                if (prefix != null && IPAddress.TryParse(prefix, out var ipPrefix))
                {
                    options.KnownIPNetworks.Add(new IPNetwork(ipPrefix, prefixLength));
                }
            }
#else
            var knownIpNetworksSection =
                configuration.GetSection($"{nameof(ForwardedHeadersOptions)}:{nameof(ForwardedHeadersOptions.KnownNetworks)}");

            options.KnownNetworks.Clear();

            foreach (var networkConfig in knownIpNetworksSection.GetChildren())
            {
                var prefix = networkConfig.GetValue<string>("Prefix");
                var prefixLength = networkConfig.GetValue<int>("PrefixLength");

                if (prefix != null && IPAddress.TryParse(prefix, out var ipPrefix))
                {
                    options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(ipPrefix, prefixLength));
                }
            }
#endif
        });

        return services;
    }

    /// <summary>
    /// Adds MVC controllers with default conventions and JSON configuration.
    /// </summary>
    /// <remarks>
    /// Registers standard response conventions for <c>401 Unauthorized</c>,
    /// <c>403 Forbidden</c>, and <c>423 Locked</c>, applies optional MVC configuration,
    /// and configures JSON serialization to write enum values as strings.
    /// </remarks>
    public static IMvcBuilder AddDefaultControllers(this IServiceCollection services)
    {
        return services.AddDefaultControllers(_ => { });
    }

    /// <summary>
    /// Adds MVC controllers with default conventions and allows custom MVC options configuration.
    /// </summary>
    /// <param name="services">The service collection to add MVC services to.</param>
    /// <param name="setupAction">
    /// An action used to configure <see cref="MvcOptions"/> in addition to the default conventions.
    /// </param>
    /// <returns>
    /// An <see cref="IMvcBuilder"/> that can be used to further configure MVC services.
    /// </returns>
    /// <remarks>
    /// Registers standard response conventions for <c>401 Unauthorized</c>,
    /// <c>403 Forbidden</c>, and <c>423 Locked</c>, and configures JSON serialization
    /// to serialize enum values as strings.
    /// </remarks>
    public static IMvcBuilder AddDefaultControllers(this IServiceCollection services, Action<MvcOptions> setupAction)
    {
        return services.AddControllers(options =>
            {
                options.Conventions.Add(new UnauthorizedResponseConvention());
                options.Conventions.Add(new ForbiddenResponseConvention());
                options.Conventions.Add(new LockedResponseConvention());

                setupAction(options);
            })
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter()));
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
            })
            .AddMvc()
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
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
}