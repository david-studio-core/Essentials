using DavidStudio.Core.Auth.PermissionAuthorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DavidStudio.Core.Auth.Conventions;

public class PermissionForbiddenResponseConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                var hasPermission = action.Attributes.Any(a => a is HasPermissionAttribute) ||
                                     controller.Attributes.Any(a => a is HasPermissionAttribute);

                if (hasPermission)
                {
                    action.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
                }
            }
        }
    }
}