using System.ComponentModel.DataAnnotations;

namespace DavidStudio.Core.Auth.Options;

public class SessionsManagementOptions
{
    [Required]
    [Url]
    public required string IdentityUrl { get; init; }
}