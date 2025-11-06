using System.Reflection;
using DavidStudio.Core.Swagger.Attributes;
using DavidStudio.Core.Swagger.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Configurations;

public class DefaultSwaggerOptions(IApiVersionDescriptionProvider provider, string title)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description, title));

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
        }
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