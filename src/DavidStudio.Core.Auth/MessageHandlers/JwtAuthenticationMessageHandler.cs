using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DavidStudio.Core.Auth.MessageHandlers;

/// <summary>
/// A <see cref="DelegatingHandler"/> that adds the JWT authorization token
/// from the current HTTP context to outgoing HTTP requests.
/// </summary>
public class JwtAuthenticationMessageHandler(IHttpContextAccessor accessor) : DelegatingHandler
{
    /// <summary>
    /// Sends an HTTP request with the JWT authorization token from the current HTTP context.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to send.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string accessToken = accessor.HttpContext?.Request.Headers[HeaderNames.Authorization]!;

        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}