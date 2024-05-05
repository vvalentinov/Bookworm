namespace Bookworm.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class PageValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (Controller)context.Controller;

            var isPresent = context.ActionArguments.TryGetValue("page", out var page);

            if (isPresent && (int)page <= 0)
            {
                var actionName = (string)context.RouteData.Values["action"];
                context.Result = controller.RedirectToAction(actionName, new { page = 1 });
            }

            base.OnActionExecuting(context);
        }
    }
}
