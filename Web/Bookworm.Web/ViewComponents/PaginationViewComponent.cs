namespace Bookworm.Web.ViewComponents
{
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string action,
            string controller,
            BaseListingViewModel model,
            bool isForBooksInCategory = false)
        {
            this.ViewData["Action"] = action;
            this.ViewData["Controller"] = controller;
            this.ViewData["IsForBooksInCategory"] = isForBooksInCategory;
            return this.View(model);
        }
    }
}
