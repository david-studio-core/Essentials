using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DavidStudio.Core.Auth.Conventions;

public class UnauthorizedResponseConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                var allowAnonymous = action.Attributes.Any(a => a is AllowAnonymousAttribute) ||
                                     controller.Attributes.Any(a => a is AllowAnonymousAttribute);

                if (!allowAnonymous)
                {
                    action.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                }
            }
        }
    }
}