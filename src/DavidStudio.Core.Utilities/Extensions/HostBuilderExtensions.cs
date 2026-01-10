using Microsoft.Extensions.Hosting;
using Serilog;

namespace DavidStudio.Core.Utilities.Extensions;

public static class HostBuilderExtensions
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
}