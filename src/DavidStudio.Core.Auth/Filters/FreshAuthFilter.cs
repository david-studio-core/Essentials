using DavidStudio.Core.Auth.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DavidStudio.Core.Auth.Filters;

public class FreshAuthFilter(long maxAgeInSeconds) : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (TokenHelper.IsAuthenticatedWithin(context.HttpContext.User, TimeSpan.FromSeconds(maxAgeInSeconds))) return;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = ErrorMessages.JwtMustBeNew
        };

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            ContentTypes = ["application/json"]
        };
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}