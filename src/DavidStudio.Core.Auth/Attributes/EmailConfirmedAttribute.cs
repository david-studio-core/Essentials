using DavidStudio.Core.Auth.Data;
using DavidStudio.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DavidStudio.Core.Auth.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class EmailConfirmedAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var claim = context.HttpContext.User.FindFirst(DavidStudioClaimTypes.EmailConfirmed);

        if (claim is null || claim.Value != true.ToString())
        {
            context.Result = new UnauthorizedObjectResult(
                OperationResult.Failure(
                    new OperationResultMessage(
                        ErrorMessages.EmailMustBeConfirmed, OperationResultSeverity.Error)));
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}