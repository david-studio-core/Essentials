namespace DavidStudio.Core.Auth.Data;

public static class DavidStudioClaimTypes
{
    internal const string ClaimTypeNamespace = "http://schemas.davidstudio.org/identity/claims/";

    public const string LicenceVersion = ClaimTypeNamespace + "licence_version";
    public const string PrivacyPolicyVersion = ClaimTypeNamespace + "privacy_polciy_version";
    
    public const string Role = ClaimTypeNamespace + "role";
    public const string Permission = ClaimTypeNamespace + "permission";

    public const string Nickname = ClaimTypeNamespace + "nickname";
    public const string Firstname = ClaimTypeNamespace + "firstname";
    public const string Lastname = ClaimTypeNamespace + "lastname";
    public const string MiddleName = ClaimTypeNamespace + "middlename";
    public const string DateOfBirth = ClaimTypeNamespace + "date_of_birth";
    public const string Gender = ClaimTypeNamespace + "gender";
    public const string Language = ClaimTypeNamespace + "language";
    public const string Country = ClaimTypeNamespace + "country";
    public const string Address = ClaimTypeNamespace + "address";
    public const string PostalCode = ClaimTypeNamespace + "postal_code";
    public const string Email = ClaimTypeNamespace + "email";
    public const string EmailConfirmed = ClaimTypeNamespace + "email_confirmed";
    public const string Phone = ClaimTypeNamespace + "phone";
    public const string PhoneConfirmed = ClaimTypeNamespace + "phone_confirmed";

    public const string SessionIdentifier = ClaimTypeNamespace + "session_identifier";
    public const string Amr = ClaimTypeNamespace + "amr";
    public const string AuthTime = ClaimTypeNamespace + "auth_time";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Iss = "iss";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Sub = "sub";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Aud = "aud";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Exp = "exp";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Nbf = "nbf";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Iat = "iat";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-4" />
    /// </summary>
    public const string Jti = "jti";

    /// <summary>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7519#section-5" />
    /// </summary>
    public const string Typ = "typ";
}