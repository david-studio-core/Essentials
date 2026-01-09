using DavidStudio.Core.Auth.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DavidStudio.Core.Auth.Extensions;

/// <summary>
/// Provides extension methods to configure SignalR hubs.
/// </summary>
public static class HubsServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IdentityIdProvider"/> as the <see cref="IUserIdProvider"/> for SignalR.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This allows SignalR to identify users by their claim-based IdentityId.
    /// </remarks>
    public static IServiceCollection AddIdentityIdProvider(this IServiceCollection services)
    {
        services.AddSingleton<IUserIdProvider, IdentityIdProvider>();

        return services;
    }
}