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

public static class ServiceCollectionExtensions
{
    public static IHostBuilder UseSerilogFromConfiguration(this IHostBuilder host)
    {
        host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));

        return host;
    }

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

    public static void AddCorsFromConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(nameof(ApplicationCorsOptions)).Get<ApplicationCorsOptions>();
        ArgumentNullException.ThrowIfNull(corsOptions);

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