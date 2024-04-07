namespace Bookworm.Web.ViewComponents
{
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string controller,
            string action,
            bool isForBooks,
            BaseListingViewModel model)
        {
            this.ViewData["Controller"] = controller;
            this.ViewData["Action"] = action;
            this.ViewData["IsForBooks"] = isForBooks;
            return this.View(model);
        }
    }
}
