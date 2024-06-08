namespace Bookworm.Web.Infrastructure.Filters
{
    using System.Collections.Generic;
    using System.Dynamic;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class PageValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isPageRoutePresent = context.ActionArguments.TryGetValue("page", out var page);

            if (isPageRoutePresent && (int)page <= 0)
            {
                context.ActionArguments["page"] = 1;

                var routeValues = new ExpandoObject();

                foreach (var kvp in context.ActionArguments)
                {
                    routeValues.TryAdd(kvp.Key, kvp.Value);
                }

                var controller = (Controller)context.Controller;
                var actionName = (string)context.RouteData.Values["action"];
                context.Result = controller.RedirectToAction(actionName, routeValues);
            }

            base.OnActionExecuting(context);
        }
    }
}
