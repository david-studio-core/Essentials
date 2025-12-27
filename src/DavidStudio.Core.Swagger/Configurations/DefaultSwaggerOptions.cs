using System.Reflection;
using Asp.Versioning.ApiExplorer;
using DavidStudio.Core.Swagger.Attributes;
using DavidStudio.Core.Swagger.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Configurations;

/// <summary>
/// Configures default Swagger generation options for an ASP.NET Core application, including API versioning, tagging, 
/// action ordering, and schema filters.
/// </summary>
/// <remarks>
/// This class implements <see cref="IConfigureOptions{SwaggerGenOptions}"/> and is designed to be used with <see cref="SwaggerGenOptions"/>.
/// It automatically sets up:
/// <list type="bullet">
/// <item>Swagger documents for all API versions provided by <see cref="IApiVersionDescriptionProvider"/>.</item>
/// <item>Action tagging by controller name.</item>
/// <item>Custom controller ordering via <see cref="SwaggerControllerOrder{TController}"/>.</item>
/// <item>Schema filtering for "strongly-typed" ID types using <see cref="SwaggerStrongIdFilter"/>.</item>
/// </list>
/// </remarks>
/// <param name="serviceProvider">The default IServiceProvider.</param>
/// <param name="title">The title to display in Swagger UI for all API versions.</param>
public class DefaultSwaggerOptions(IServiceProvider serviceProvider, string title)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.EnableAnnotations();
        options.TagActionsBy(api =>
        {
            if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                return [controllerActionDescriptor.ControllerName];

            throw new InvalidOperationException("Unable to determine tag for endpoint.");
        });

        SwaggerControllerOrder<ControllerBase> swaggerControllerOrder = new(Assembly.GetEntryAssembly() ?? throw new NullReferenceException());
        options.OrderActionsBy(apiDesc => swaggerControllerOrder.SortKey(apiDesc.ActionDescriptor.RouteValues["controller"]));

        options.SchemaFilter<SwaggerStrongIdFilter>();

        using var scope = serviceProvider.CreateScope();
        var apiVersionDescriptionProvider = scope.ServiceProvider.GetService<IApiVersionDescriptionProvider>();
        if (apiVersionDescriptionProvider is null) return;

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description, title));
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription desc, string title)
    {
        var info = new OpenApiInfo
        {
            Title = title,
            Version = desc.ApiVersion.ToString()
        };

        if (desc.IsDeprecated)
            info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";

        return info;
    }
}