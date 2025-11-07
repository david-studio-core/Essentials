using Microsoft.AspNetCore.Authorization;

namespace DavidStudio.Core.Auth.PermissionAuthorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
