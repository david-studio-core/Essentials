using Asp.Versioning.ApiExplorer;
using DavidStudio.Core.Swagger.Configurations;
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
    /// Registers Swagger services with default configuration and optional bearer token authentication support.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Swagger services to.</param>
    /// <param name="title">The title of the API to be displayed in Swagger UI.</param>
    /// <param name="bearer">
    /// If <c>true</c>, configures Swagger to include bearer token authentication in the UI. 
    /// Defaults to <c>false</c>.
    /// </param>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item>Registers "EndpointsApiExplorer" to enable API endpoint discovery.</item>
    /// <item>Adds Swagger generator services ("SwaggerGen").</item>
    /// <item>Configures default Swagger options through <see cref="DefaultSwaggerOptions"/> using API versioning.</item>
    /// <item>If <paramref name="bearer"/> is <c>true</c>, configures bearer authentication options for Swagger UI.</item>
    /// </list>
    /// </remarks>
    public static void AddDefaultSwagger(this IServiceCollection services, string title, bool bearer = false)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>>(sp =>
        {
            var provider = sp.GetRequiredService<IServiceProvider>();
            return new DefaultSwaggerOptions(provider, title);
        });

        if (bearer)
            services.ConfigureOptions<BearerAuthenticationSwaggerOptions>();
    }

    /// <summary>
    /// Enables and configures Swagger middleware in the ASP.NET Core request pipeline with default UI and versioning support.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to configure Swagger for.</param>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item>Retrieves <see cref="IApiVersionDescriptionProvider"/> to enumerate API versions.</item>
    /// <item>Enables the Swagger middleware using "IApplicationBuilder.UseSwagger()".</item>
    /// <item>Configures Swagger UI endpoints for all API versions in reverse order (latest first).</item>
    /// <item>Maps the root path "/" to redirect to "/swagger" and excludes it from API documentation.</item>
    /// </list>
    /// </remarks>
    public static void UseDefaultSwagger(this WebApplication app)
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
        });

        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    }
}