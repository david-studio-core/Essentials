using DavidStudio.Core.Auth.Data;

namespace DavidStudio.Core.Auth.Hubs;

public class ProfileIdProvider : IUserIdProvider
{
  public virtual string? GetUserId(HubConnectionContext connection)
  {
    return connection.User.FindFirst(ApplicationClaimTypes.ProfileIdentifier)?.Value;
  }
}
