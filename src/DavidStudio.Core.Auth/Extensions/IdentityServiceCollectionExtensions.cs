using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DavidStudio.Core.Auth.Data;
using DavidStudio.Core.Auth.Options;
using DavidStudio.Core.Auth.PermissionAuthorization;
using DavidStudio.Core.Auth.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DavidStudio.Core.Auth.Extensions;

/// <summary>
/// Provides extension methods to configure JWT authentication and permission-based authorization.
/// </summary>
public static class IdentityServiceCollectionExtensions
{
    /// <summary>
    /// Configures JWT authentication for the application using settings from configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add authentication to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing JWT settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddJwtAuthentication(configuration, []);
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
    public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration, string[] hubs)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(nameof(JwtOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtAuthorizationOptions = configuration
                                          .GetRequiredSection(nameof(JwtOptions))
                                          .Get<JwtOptions>()
                                      ?? throw new InvalidOperationException($"Missing configuration section: {nameof(JwtOptions)}");

        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtAuthorizationOptions.Issuer,
                ValidAudience = jwtAuthorizationOptions.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthorizationOptions.SecretKey)),

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ClockSkew = TimeSpan.FromSeconds(30),

                RoleClaimType = DavidStudioClaimTypes.Role,
                NameClaimType = DavidStudioClaimTypes.Nickname
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
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = context.ErrorDescription ??
                                 context.AuthenticateFailure?.Message ??
                                 "You are not authorized to access this resource."
                    };

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                },
                OnTokenValidated = context =>
                {
                    var typ = context.Principal?.FindFirstValue(DavidStudioClaimTypes.Typ);
                    if (typ == "2fa_challenge")
                        context.Fail("2FA challenge token is not valid for API access.");

                    return Task.CompletedTask;
                }
            };
        });
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