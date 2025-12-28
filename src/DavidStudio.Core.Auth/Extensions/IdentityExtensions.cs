using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DavidStudio.Core.Auth.Data;
using DavidStudio.Core.Auth.Options;
using DavidStudio.Core.Auth.PermissionAuthorization;
using DavidStudio.Core.Auth.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DavidStudio.Core.Auth.Extensions;

/// <summary>
/// Provides extension methods to configure JWT authentication and permission-based authorization.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Configures JWT authentication for the application using settings from configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add authentication to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing JWT settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        return AddJwtAuthentication(services, configuration, []);
    }

    /// <summary>
    /// Configures JWT authentication for the application using settings from configuration
    /// and optionally enables token support for SignalR hubs.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add authentication to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing JWT settings.</param>
    /// <param name="hubs">An array of SignalR hub paths for which JWT tokens should be read from HTTP headers.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the JWT configuration section is missing.</exception>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration, string[] hubs)
    {
        services.AddOptions<JwtAuthorizationOptions>()
            .BindConfiguration(nameof(JwtAuthorizationOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtAuthorizationOptions = configuration
                                          .GetRequiredSection(nameof(JwtAuthorizationOptions))
                                          .Get<JwtAuthorizationOptions>()
                                      ?? throw new InvalidOperationException("Missing configuration section: JwtAuthorizationOptions");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtAuthorizationOptions.Issuer,
                ValidAudience = jwtAuthorizationOptions.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthorizationOptions.SecretKey)),

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                
                RoleClaimType = DavidStudioClaimTypes.Role,
                NameClaimType = DavidStudioClaimTypes.Sub
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (hubs.Length == 0)
                        return Task.CompletedTask;

                    var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(authHeader) ||
                        !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.CompletedTask;
                    }

                    var accessToken = authHeader["Bearer ".Length..].Trim();
                    var path = context.HttpContext.Request.Path;

                    foreach (var hub in hubs)
                    {
                        if (path.StartsWithSegments(hub))
                            context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                OnTokenValidated = ctx =>
                {
                    var typ = ctx.Principal?.FindFirstValue(JwtRegisteredClaimNames.Typ);
                    if (typ == "2fa_challenge")
                        ctx.Fail("2FA challenge token is not valid for API access.");

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Registers permission-based authorization services for production use.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.ConfigureOptions<SwaggerPermissionsOptions>();

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    /// <summary>
    /// Registers stub permission-based authorization services for testing or development purposes.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddPermissionAuthorizationStub(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandlerStub>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}