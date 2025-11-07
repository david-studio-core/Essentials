using System.Text;
using DavidStudio.Core.Auth.Options;
using DavidStudio.Core.Auth.PermissionAuthorization;
using DavidStudio.Core.Auth.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DavidStudio.Core.Auth.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        return AddJwtAuthentication(services, configuration, []);
    }

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
                ValidateIssuerSigningKey = true
            };

            if (hubs.Length > 0)
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
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
                    }
                };
            }
        });

        return services;
    }

    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.ConfigureOptions<SwaggerPermissionsOptions>();

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    public static IServiceCollection AddPermissionAuthorizationStub(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandlerStub>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}