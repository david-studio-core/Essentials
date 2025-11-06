namespace DavidStudio.Core.Auth.Services;

public class SessionsService(HttpClient httpClient)
{
    public async Task<bool> IsExpiredAsync()
    {
        var response = await httpClient.GetStringAsync("api/Sessions/check");

        return bool.Parse(response);
    }
}