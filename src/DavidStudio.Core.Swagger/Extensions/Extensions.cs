using Asp.Versioning.ApiExplorer;
using DavidStudio.Core.Swagger.Configurations;
using DavidStudio.Core.Swagger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Extensions;

/// <summary>
/// Provides extension methods for registering and configuring Swagger in an ASP.NET Core application with default settings.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers Swagger services with default configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Swagger services to.</param>
    /// <param name="title">The title of the API to be displayed in Swagger UI.</param>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item>Registers "EndpointsApiExplorer" to enable API endpoint discovery.</item>
    /// <item>Adds Swagger generator services ("SwaggerGen").</item>
    /// <item>Configures default Swagger options through <see cref="DefaultSwaggerOptions"/> using API versioning.</item>
    /// </list>
    /// </remarks>
    public static void AddDefaultSwagger(this IServiceCollection services, string title)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.UseAllOfToExtendReferenceSchemas();
            options.SupportNonNullableReferenceTypes();
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>>(sp =>
        {
            var provider = sp.GetRequiredService<IServiceProvider>();
            return new DefaultSwaggerOptions(provider, title);
        });
    }

    /// <summary>
    /// Registers Swagger services with default configuration and bearer token authentication support.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Swagger services to.</param>
    /// <param name="title">The title of the API to be displayed in Swagger UI.</param>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item>Registers "EndpointsApiExplorer" to enable API endpoint discovery.</item>
    /// <item>Adds Swagger generator services ("SwaggerGen").</item>
    /// <item>Configures default Swagger options through <see cref="DefaultSwaggerOptions"/> using API versioning.</item>
    /// </list>
    /// </remarks>
    public static void AddSwaggerWithBearer(this IServiceCollection services, string title)
    {
        services.AddDefaultSwagger(title);

        services.ConfigureOptions<BearerAuthenticationSwaggerOptions>();
    }

    /// <summary>
    /// Registers Swagger services with default configuration and bearer token authentication support.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Swagger services to.</param>
    /// <param name="title">The title of the API to be displayed in Swagger UI.</param>
    /// <param name="authorizationUrl">The authorization url from OAuth.</param>
    /// <param name="tokenUrl">The token url from OAuth.</param>
    /// <param name="scopes">Scopes to be included in swagger.</param>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item>Registers "EndpointsApiExplorer" to enable API endpoint discovery.</item>
    /// <item>Adds Swagger generator services ("SwaggerGen").</item>
    /// <item>Configures default Swagger options through <see cref="DefaultSwaggerOptions"/> using API versioning.</item>
    /// </list>
    /// </remarks>
    public static void AddSwaggerWithOAuth2(this IServiceCollection services, string title, string authorizationUrl, string tokenUrl, string[] scopes)
    {
        services.AddDefaultSwagger(title);

        services.Configure<OAuth2CodeFlowOptions>(options =>
        {
            options.AuthorizationUrl = authorizationUrl;
            options.TokenUrl = tokenUrl;
            options.Scopes = scopes;
        });

        services.ConfigureOptions<OAuth2AuthenticationSwaggerOptions>();
    }

    /// <summary>
    /// Enables and configures Swagger middleware in the ASP.NET Core request pipeline with default UI and versioning support.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to configure Swagger for.</param>
    /// <param name="usePcke">Determines if swagger should use PKCE for OAuth</param>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item>Retrieves <see cref="IApiVersionDescriptionProvider"/> to enumerate API versions.</item>
    /// <item>Enables the Swagger middleware using "IApplicationBuilder.UseSwagger()".</item>
    /// <item>Configures Swagger UI endpoints for all API versions in reverse order (latest first).</item>
    /// <item>Maps the root path "/" to redirect to "/swagger" and excludes it from API documentation.</item>
    /// </list>
    /// </remarks>
    public static void UseDefaultSwagger(this WebApplication app, bool usePcke = false)
    {
        var apiVersionDescriptionProvider = app.Services.GetService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            if (apiVersionDescriptionProvider is not null)
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            }

            if (usePcke)
                options.OAuthUsePkce();
        });

        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    }
}