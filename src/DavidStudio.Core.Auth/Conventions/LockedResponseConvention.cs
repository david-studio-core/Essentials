using DavidStudio.Core.Auth.Attributes;
using DavidStudio.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DavidStudio.Core.Auth.Conventions;

public class LockedResponseConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                var locked = action.Attributes.Any(a => a is EmailConfirmedAttribute) ||
                             controller.Attributes.Any(a => a is EmailConfirmedAttribute) ||
                             action.Attributes.Any(a => a is PhoneConfirmedAttribute) ||
                             controller.Attributes.Any(a => a is PhoneConfirmedAttribute);

                if (locked)
                {
                    action.Filters.Add(new ProducesResponseTypeAttribute<OperationResult>(StatusCodes.Status423Locked));
                }
            }
        }
    }
}