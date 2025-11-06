using DavidStudio.Core.Swagger.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Extensions;

public static class Extensions
{
    public static void AddDefaultSwagger(this IServiceCollection services, string title, bool bearer = false)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>>(sp =>
        {
            var provider = sp.GetRequiredService<IApiVersionDescriptionProvider>();
            return new DefaultSwaggerOptions(provider, title);
        });

        if (bearer)
            services.ConfigureOptions<BearerAuthenticationSwaggerOptions>();
    }

    public static void UseDefaultSwagger(this WebApplication app)
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    }
}