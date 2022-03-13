namespace Bookworm.Web.Controllers
{
    using Bookworm.Common;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class BookController : Controller
    {
        private readonly IBooksService booksService;

        public BookController(IBooksService booksService)
        {
            this.booksService = booksService;
        }

        [Authorize]
        public IActionResult Upload()
        {
            return this.View(new UploadBookFormModel()
            {
                Categories = this.booksService.GetBookCategories<CategoryViewModel>(),
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Upload(UploadBookFormModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added book!";
            return this.RedirectToAction("Index", "Home");
        }
    }
}
