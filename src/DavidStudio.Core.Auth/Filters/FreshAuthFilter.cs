using DavidStudio.Core.Auth.Utilities;
using DavidStudio.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DavidStudio.Core.Auth.Filters;

public class FreshAuthFilter(long maxAgeInSeconds) : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (!TokenHelper.IsAuthenticatedWithin(context.HttpContext.User, TimeSpan.FromSeconds(maxAgeInSeconds)))
        {
            context.Result = new UnauthorizedObjectResult(
                OperationResult.Failure(
                    new OperationResultMessage(
                        ErrorMessages.JwtMustBeNew, OperationResultSeverity.Error)));
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}