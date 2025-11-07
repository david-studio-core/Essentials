using System.Security.Claims;

namespace DavidStudio.Core.Auth.Data;

public static class ApplicationClaimTypes
{
    public const string UserIdentifier = ClaimTypes.NameIdentifier;
    public const string UserSessionIdentifier = "http://schemas.davidstudio.core.essentials.com/identity/claims/usersessionidentifier";
    public const string Permission = "http://schemas.davidstudio.core.essentials.com/identity/claims/permission";
    public const string Email = ClaimTypes.Email;
    public const string EmailConfirmed = "http://schemas.davidstudio.core.essentials.com/identity/claims/emailconfirmed";
    public const string ProfileIdentifier = "http://schemas.davidstudio.core.essentials.com/identity/claims/profileidentifier";
    public const string Nickname = "http://schemas.davidstudio.core.essentials.com/identity/claims/nickname";
    public const string Firstname = ClaimTypes.GivenName;
    public const string Lastname = ClaimTypes.Surname;
    public const string DateOfBirth = ClaimTypes.DateOfBirth;
    public const string Sex = "http://schemas.davidstudio.core.essentials.com/identity/claims/sex";
}