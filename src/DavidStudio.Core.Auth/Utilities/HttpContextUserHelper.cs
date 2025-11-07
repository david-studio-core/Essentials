using System.Security.Claims;
using DavidStudio.Core.Auth.Data;
using DavidStudio.Core.Auth.Enums;
using DavidStudio.Core.Auth.StronglyTypedIds;

namespace DavidStudio.Core.Auth.Utilities;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/> to easily
/// extract application-specific user information from claims.
/// </summary>
public static class HttpContextUserHelper
{
    /// <summary>
    /// Retrieves the user's unique IdentityId from claims.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The <see cref="IdentityId"/> extracted from the claims.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="ApplicationClaimTypes.UserIdentifier"/> claim is missing.
    /// </exception>
    public static IdentityId GetId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ApplicationClaimTypes.UserIdentifier)
                     ?? throw new InvalidOperationException($"{ApplicationClaimTypes.UserIdentifier} is required");

        return IdentityId.Parse(userId);
    }

    /// <summary>
    /// Retrieves the current user SessionId from claims.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The <see cref="UserSessionId"/> extracted from the claims.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="ApplicationClaimTypes.UserSessionIdentifier"/> claim is missing.
    /// </exception>
    public static UserSessionId GetSessionId(this ClaimsPrincipal user)
    {
        var sessionId = user.FindFirstValue(ApplicationClaimTypes.UserSessionIdentifier)
                        ?? throw new InvalidOperationException($"{ApplicationClaimTypes.UserSessionIdentifier} is required");

        return UserSessionId.Parse(sessionId);
    }

    /// <summary>
    /// Retrieves the user's date of birth from claims.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The user's date of birth as <see cref="DateTime"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="ApplicationClaimTypes.DateOfBirth"/> claim is missing.
    /// </exception>
    public static DateTime GetDateOfBirth(this ClaimsPrincipal user)
    {
        var dateOfBirth = user.FindFirstValue(ApplicationClaimTypes.DateOfBirth)
                          ?? throw new InvalidOperationException($"{ApplicationClaimTypes.DateOfBirth} is required");

        return DateTime.Parse(dateOfBirth);
    }

    /// <summary>
    /// Retrieves the user's sex from claims.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The <see cref="Sex"/> enum value extracted from claims.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="ApplicationClaimTypes.Sex"/> claim is missing.
    /// </exception>
    public static Sex GetSex(this ClaimsPrincipal user)
    {
        var sex = user.FindFirstValue(ApplicationClaimTypes.Sex)
                  ?? throw new InvalidOperationException($"{ApplicationClaimTypes.Sex} is required");

        return Enum.Parse<Sex>(sex, ignoreCase: true);
    }

    /// <summary>
    /// Retrieves a required claim value from the <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <param name="claimType">The claim type to retrieve.</param>
    /// <returns>The claim value as a string.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified claim is missing.
    /// </exception>
    public static string GetRequiredClaim(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirstValue(claimType)
               ?? throw new InvalidOperationException($"{claimType} is required");
    }
}