using DavidStudio.Core.Auth.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DavidStudio.Core.Auth.Extensions;

public static class HubsExtensions
{
    public static IServiceCollection AddProfileIdProvider(this IServiceCollection services)
    {
        services.AddSingleton<IUserIdProvider, ProfileIdProvider>();

        return services;
    }
}