namespace DavidStudio.Core.Auth.Services;

/// <summary>
/// Provides functionality to manage and validate user sessions.
/// </summary>
public class SessionsService(HttpClient httpClient)
{
    /// <summary>
    /// Checks whether the current user's session has expired by calling the remote session API.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{Boolean}"/> that resolves to <c>true</c> if the session is expired; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExpiredAsync()
    {
        var response = await httpClient.GetStringAsync("api/Sessions/check");

        return bool.Parse(response);
    }
}