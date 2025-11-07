using Microsoft.AspNetCore.Authorization;

namespace DavidStudio.Core.Auth.PermissionAuthorization;

/// <summary>
/// Specifies that a controller or action requires one or more permissions.
/// Inherits from <see cref="AuthorizeAttribute"/> and sets the policy based on the provided permissions.
/// </summary>
/// <example>
/// The following example shows how to apply the attribute to a controller action:
/// <code>
/// [HasPermission("Users.Read", "Users.Manage")]
/// public IActionResult GetUsers()
/// {
///     // Action logic here
/// }
/// </code>
/// In this example, the action can only be executed by users who have the
/// "Users.Read" and "Users.Manage" permissions.
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class HasPermissionAttribute(params string[] permissions)
    : AuthorizeAttribute(policy: string.Join(",", permissions));
