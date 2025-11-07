using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DavidStudio.Core.Auth.MessageHandlers;

public class JwtAuthenticationMessageHandler(IHttpContextAccessor accessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string accessToken = accessor.HttpContext?.Request.Headers[HeaderNames.Authorization]!;

        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
