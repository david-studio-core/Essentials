using Microsoft.AspNetCore.Authorization;

namespace DavidStudio.Core.Auth.PermissionAuthorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class HasPermissionAttribute(params string[] permissions)
    : AuthorizeAttribute(policy: string.Join(",", permissions));
