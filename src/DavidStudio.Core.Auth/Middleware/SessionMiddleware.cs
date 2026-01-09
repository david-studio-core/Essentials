using System.Text.Json;
using DavidStudio.Core.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DavidStudio.Core.Auth.Middleware;

/// <summary>
/// Middleware that checks the current user's session for expiration and
/// returns 401 Unauthorized if the session has expired.
/// </summary>
public class SessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, SessionsService sessionsService)
    {
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated &&
            await sessionsService.IsExpiredAsync())
        {
            
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Your session has expired."
            };
            
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(problemDetails);
            
            await context.Response.WriteAsync(json);

            return;
        }

        await next(context);
    }
}