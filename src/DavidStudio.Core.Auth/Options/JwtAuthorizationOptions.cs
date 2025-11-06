using System.ComponentModel.DataAnnotations;

namespace DavidStudio.Core.Auth.Options;

public class JwtAuthorizationOptions
{
    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    [Required]
    public string SecretKey { get; init; } = null!;
}
