namespace Bookworm.Web.Controllers
{
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Mvc;

    public class BookController : Controller
    {
        private readonly IBooksService booksService;

        public BookController(IBooksService booksService)
        {
            this.booksService = booksService;
        }

        public IActionResult Upload()
        {
            return this.View(new UploadBookFormModel()
            {
                Categories = this.booksService.GetBookCategories<CategoryViewModel>(),
            });
        }
    }
}
