using System.Security.Claims;
using DavidStudio.Core.Auth.Data;

namespace DavidStudio.Core.Auth.Utilities;

public static class TokenHelper
{
    public static bool IsAuthenticatedWithin(ClaimsPrincipal user, TimeSpan maxAge, DateTimeOffset? now = null)
    {
        now ??= DateTimeOffset.UtcNow;

        var authTimeStr = user.FindFirstValue(DavidStudioClaimTypes.AuthTime);
        if (authTimeStr is null || !long.TryParse(authTimeStr, out var autoTimeSeconds))
            return false;

        var authTime = DateTimeOffset.FromUnixTimeSeconds(autoTimeSeconds);

        var age = now.Value - authTime;

        if (age < TimeSpan.Zero) return false;

        return age <= maxAge;
    }
}