using DavidStudio.Core.Auth.Hubs;

namespace DavidStudio.Core.Auth.Extensions;

public static class HubsExtensions
{
    public static IServiceCollection AddSignalRProfileIdProvider(this IServiceCollection services)
    {
        services.AddSingleton<IUserIdProvider, ProfileIdProvider>();

        return services;
    }
}