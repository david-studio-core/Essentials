using DavidStudio.Core.Auth.Data;
using Microsoft.AspNetCore.SignalR;

namespace DavidStudio.Core.Auth.Hubs;

/// <summary>
/// Provides a custom user ID for SignalR connections based on the user's claims.
/// </summary>
/// <remarks>
/// This implementation retrieves the user ID from the claim defined in
/// <see cref="ApplicationClaimTypes.UserIdentifier"/> and can be used to
/// uniquely identify users in SignalR hubs.
/// </remarks>
public class IdentityIdProvider : IUserIdProvider
{
    /// <summary>
    /// Retrieves the user ID for the specified SignalR connection.
    /// </summary>
    /// <param name="connection">The <see cref="HubConnectionContext"/> representing the SignalR connection.</param>
    /// <returns>
    /// The user ID as a <see cref="string"/> extracted from the <see cref="ApplicationClaimTypes.UserIdentifier"/> claim,
    /// or <c>null</c> if the claim is not present.
    /// </returns>
    public virtual string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst(ApplicationClaimTypes.UserIdentifier)?.Value;
    }
}