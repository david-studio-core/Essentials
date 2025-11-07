using Microsoft.AspNetCore.Authorization;

namespace DavidStudio.Core.Auth.PermissionAuthorization;

public class PermissionAuthorizationHandlerStub : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
