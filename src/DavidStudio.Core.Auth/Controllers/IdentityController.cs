using DavidStudio.Core.Auth.Enums;
using DavidStudio.Core.Auth.StronglyTypedIds;
using DavidStudio.Core.Auth.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DavidStudio.Core.Auth.Controllers;

/// <summary>
/// Base controller providing helper methods to access the current authenticated user's identity information.
/// </summary>
/// <remarks>
/// This controller is intended to be inherited by other API controllers that require
/// access to user-specific claims, such as user ID, session ID, date of birth, or sex.
/// </remarks>
public class IdentityController : ControllerBase
{
    /// <summary>
    /// Gets the current authenticated user's unique identity ID.
    /// </summary>
    /// <returns>The <see cref="IdentityId"/> of the current user.</returns>
    protected IdentityId GetCurrentUserId() => HttpContext.User.GetId();

    /// <summary>
    /// Gets the current authenticated user's session ID.
    /// </summary>
    /// <returns>The <see cref="UserSessionId"/> of the current session.</returns>
    protected UserSessionId GetCurrentUserSessionId() => HttpContext.User.GetSessionId();

    /// <summary>
    /// Gets the current authenticated user's date of birth.
    /// </summary>
    /// <returns>A <see cref="DateTime"/> representing the user's date of birth.</returns>
    protected DateTime GetCurrentUserDateOfBirth() => HttpContext.User.GetDateOfBirth();

    /// <summary>
    /// Gets the current authenticated user's sex.
    /// </summary>
    /// <returns>A <see cref="Sex"/> enum value representing the user's sex.</returns>
    protected Sex GetCurrentUserSex() => HttpContext.User.GetSex();

    /// <summary>
    /// Retrieves a required claim value for the current user.
    /// </summary>
    /// <param name="claimType">The claim type to retrieve.</param>
    /// <returns>The claim value as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the claim is not present.</exception>
    protected string GetRequiredClaim(string claimType) => HttpContext.User.GetRequiredClaim(claimType);
}