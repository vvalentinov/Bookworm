namespace Bookworm.Web.Infrastructure.Filters
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class QuoteValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (Controller)context.Controller;
            var model = context.ActionArguments["model"];
            bool isEditAction = (string)context.RouteData.Values["action"] == "Edit";

            if (isEditAction)
            {
                var id = model.GetType().GetProperty("Id").GetValue(model);

                if ((int)id <= 0)
                {
                    controller.TempData[ErrorMessage] = "The quote id must be a positive number!";
                    context.Result = controller.RedirectToAction("UserQuotes");
                }
            }

            if (context.ModelState.IsValid == false)
            {
                if (isEditAction == false)
                {
                    var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                    controller.TempData[ErrorMessage] = string.Join(", ", errors);
                }

                context.Result = controller.View(model);
            }

            base.OnActionExecuting(context);
        }
    }
}
