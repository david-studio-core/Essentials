using DavidStudio.Core.Auth.PermissionAuthorization;

namespace DavidStudio.Core.Auth.Swagger;

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