using DavidStudio.Core.Auth.Data;
using Microsoft.AspNetCore.Authorization;

namespace DavidStudio.Core.Auth.PermissionAuthorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context.User.Claims
           .Where(r => r.Type == DavidStudioClaimTypes.Permission)
           .Select(r => r.Value)
           .ToHashSet();

        if (permissions.Any(permission => permission == requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
