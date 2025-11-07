using DavidStudio.Core.Auth.Services;
using Microsoft.AspNetCore.Http;

namespace DavidStudio.Core.Auth.Middleware;

public class SessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, SessionsService sessionsService)
    {
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated &&
            await sessionsService.IsExpiredAsync())
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.CompleteAsync();

            return;
        }

        await next(context);
    }
}