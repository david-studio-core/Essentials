using DavidStudio.Core.Auth.Data;
using Microsoft.AspNetCore.SignalR;

namespace DavidStudio.Core.Auth.Hubs;

public class ProfileIdProvider : IUserIdProvider
{
  public virtual string? GetUserId(HubConnectionContext connection)
  {
    return connection.User.FindFirst(ApplicationClaimTypes.ProfileIdentifier)?.Value;
  }
}
