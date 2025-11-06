namespace DavidStudio.Core.Auth.Controllers;

public class ApplicationControllerBase : ControllerBase
{
    protected IdentityId GetCurrentUserId() => HttpContext.User.GetId();
    protected UserSessionId GetCurrentUserSessionId() => HttpContext.User.GetSessionId();
    protected ProfileId GetCurrentProfileId() => HttpContext.User.GetProfileId();
    protected DateTime GetCurrentUserDateOfBirth() => HttpContext.User.GetDateOfBirth();
    protected Sex GetCurrentUserSex() => HttpContext.User.GetSex();
    protected string GetRequiredClaim(string claimType) => HttpContext.User.GetRequiredClaim(claimType);
}