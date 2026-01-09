using DavidStudio.Core.Auth.MessageHandlers;
using DavidStudio.Core.Auth.Middleware;
using DavidStudio.Core.Auth.Options;
using DavidStudio.Core.Auth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace DavidStudio.Core.Auth.Extensions;

/// <summary>
/// Provides extension methods to configure and use session management in the application.
/// </summary>
public static class SessionsServiceCollectionExtensions
{
    /// <summary>
    /// Registers services required for session management, including <see cref="SessionsService"/>
    /// and HTTP resilience policies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing session management settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Identity URL is not configured in <see cref="SessionsManagementOptions"/>.</exception>
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

    /// <summary>
    /// Adds the <see cref="SessionMiddleware"/> to the application's request pipeline
    /// to enable session management.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder UseSessionsManagement(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionMiddleware>();
    }
}