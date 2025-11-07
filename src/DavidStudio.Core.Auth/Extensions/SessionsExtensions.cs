using DavidStudio.Core.Auth.MessageHandlers;
using DavidStudio.Core.Auth.Middleware;
using DavidStudio.Core.Auth.Options;
using DavidStudio.Core.Auth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace DavidStudio.Core.Auth.Extensions;

public static class SessionsExtensions
{
    public static IServiceCollection AddSessionsManagement(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<JwtAuthenticationMessageHandler>();

        services.AddHttpClient<SessionsService>(client =>
            {
                const string key = $"{nameof(SessionsManagementOptions)}:{nameof(SessionsManagementOptions.IdentityUrl)}";
                var url = configuration.GetValue<string>(key)
                          ?? throw new InvalidOperationException($"{key} is not configured.");

                client.BaseAddress = new Uri(url);
            })
            .AddHttpMessageHandler<JwtAuthenticationMessageHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 5;
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;

                options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
            });

        return services;
    }

    public static IApplicationBuilder UseSessionsManagement(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionMiddleware>();
    }
}