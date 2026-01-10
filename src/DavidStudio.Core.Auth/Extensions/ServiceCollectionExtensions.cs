using System.Net;
using DavidStudio.Core.Auth.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DavidStudio.Core.Auth.Extensions;

public static class ServiceCollectionExtensions
{
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

    public static IMvcBuilder AddDefaultConfiguredControllers(this IServiceCollection services)
    {
        return services.AddControllers(options =>
            {
                options.Conventions.Add(new UnauthorizedResponseConvention());
                options.Conventions.Add(new ForbiddenResponseConvention());
                options.Conventions.Add(new LockedResponseConvention());
            })
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter()));
    }
}