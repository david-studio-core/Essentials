
using DavidStudio.Core.Auth.PermissionAuthorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Auth.Swagger;

/// <summary>
/// Adds a description of required permissions to Swagger/OpenAPI documentation for controller actions.
/// </summary>
/// <remarks>
/// This filter inspects the <see cref="HasPermissionAttribute"/> applied to actions or controllers,
/// extracts the required permissions, and appends them to the operation description in Swagger.
/// </remarks>
public class OperationPermissionsFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var permissions = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<HasPermissionAttribute>()
            .Select(a => a.Policy)
            .Distinct()
            .ToArray();

        if (permissions.Length == 0)
            permissions = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<HasPermissionAttribute>()
                .Select(a => a.Policy)
                .Distinct()
                .ToArray();

        if (permissions is null || permissions.Length <= 0) return;

        var permissionsString = string.Join(", ", permissions);
        operation.Description += $"<p>Required Permissions ({permissionsString})</p>";
    }
}