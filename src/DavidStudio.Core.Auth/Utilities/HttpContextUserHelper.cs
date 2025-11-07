using System.Security.Claims;
using DavidStudio.Core.Auth.Data;
using DavidStudio.Core.Auth.Enums;
using DavidStudio.Core.Auth.StronglyTypedIds;

namespace DavidStudio.Core.Auth.Utilities;

public static class HttpContextUserHelper
{
    public static IdentityId GetId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ApplicationClaimTypes.UserIdentifier)
                     ?? throw new InvalidOperationException($"{ApplicationClaimTypes.UserIdentifier} is required");

        return IdentityId.Parse(userId);
    }

    public static UserSessionId GetSessionId(this ClaimsPrincipal user)
    {
        var sessionId = user.FindFirstValue(ApplicationClaimTypes.UserSessionIdentifier)
                        ?? throw new InvalidOperationException($"{ApplicationClaimTypes.UserSessionIdentifier} is required");

        return UserSessionId.Parse(sessionId);
    }

    public static ProfileId GetProfileId(this ClaimsPrincipal user)
    {
        var profileId = user.FindFirstValue(ApplicationClaimTypes.ProfileIdentifier)
                        ?? throw new InvalidOperationException($"{ApplicationClaimTypes.ProfileIdentifier} is required");

        return ProfileId.Parse(profileId);
    }

    public static DateTime GetDateOfBirth(this ClaimsPrincipal user)
    {
        var dateOfBirth = user.FindFirstValue(ApplicationClaimTypes.DateOfBirth)
                          ?? throw new InvalidOperationException($"{ApplicationClaimTypes.DateOfBirth} is required");

        return DateTime.Parse(dateOfBirth);
    }

    public static Sex GetSex(this ClaimsPrincipal user)
    {
        var sex = user.FindFirstValue(ApplicationClaimTypes.Sex)
                  ?? throw new InvalidOperationException($"{ApplicationClaimTypes.Sex} is required");

        return Enum.Parse<Sex>(sex, ignoreCase: true);
    }

    public static string GetRequiredClaim(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirstValue(claimType)
               ?? throw new InvalidOperationException($"{claimType} is required");
    }
}