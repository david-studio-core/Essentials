using System.ComponentModel.DataAnnotations;

namespace DavidStudio.Core.Auth.Options;

public class SessionsManagementOptions
{
    [Required]
    public required string IdentityUrl { get; init; }
}