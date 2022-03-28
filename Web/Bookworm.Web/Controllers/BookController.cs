namespace Bookworm.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class BookController : Controller
    {
        private readonly IBooksService booksService;
        private readonly ICategoriesService categoriesService;
        private readonly IUploadBookService uploadBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILanguagesService languagesService;
        private readonly IFavoriteBooksService favoriteBooksService;
        private readonly IBlobService blobService;

        public BookController(
            IBooksService booksService,
            ICategoriesService categoriesService,
            IUploadBookService uploadBookService,
            UserManager<ApplicationUser> userManager,
            ILanguagesService languagesService,
            IFavoriteBooksService favoriteBooksService,
            IBlobService blobService)
        {
            this.booksService = booksService;
            this.categoriesService = categoriesService;
            this.uploadBookService = uploadBookService;
            this.userManager = userManager;
            this.languagesService = languagesService;
            this.favoriteBooksService = favoriteBooksService;
            this.blobService = blobService;
        }

        public IActionResult All(string categoryName, int page = 1)
        {
            int categoryId = this.categoriesService.GetCategoryId(categoryName);

            var model = this.booksService.GetBooks(categoryId, page, 12);
            return this.View(model);
        }

        public async Task<IActionResult> CurrentBook(string id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            BookViewModel bookViewModel = this.booksService.GetBookWithId(id, user == null ? null : user.Id);
            return this.View(bookViewModel);
        }

        [Authorize]
        public async Task<IActionResult> AddToFavorites(string bookId)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            try
            {
                await this.favoriteBooksService.AddBookToFavoritesAsync(bookId, user.Id);
                this.TempData[MessageConstant.SuccessMessage] = "Successfully added book to favorites!";
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.WarningMessage] = ex.Message;
                return this.RedirectToAction("CurrentBook", new { id = bookId });
            }

            return this.RedirectToAction("CurrentBook", new { id = bookId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteFromFavorites(string bookId)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            await this.favoriteBooksService.DeleteFromFavoritesAsync(bookId, user.Id);
            this.TempData[MessageConstant.SuccessMessage] = "Successfully removed book!";
            return this.RedirectToAction("Favorites");
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            var books = this.favoriteBooksService.GetUserFavoriteBooks(user.Id);
            return this.View(books);
        }

        [Authorize]
        public IActionResult Upload()
        {
            return this.View(new UploadBookFormModel()
            {
                Categories = this.booksService.GetBookCategories(),
                Languages = this.languagesService.GetAllLanguages(),
            });
        }

        [Authorize]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload(UploadBookFormModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                model.Categories = this.booksService.GetBookCategories();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            try
            {
                await this.uploadBookService.UploadBookAsync(
                    model.Title,
                    model.Description,
                    model.LanguageId,
                    model.Publisher,
                    model.PagesCount,
                    model.PublishedYear,
                    model.BookFile,
                    model.ImageFile,
                    model.CategoryId,
                    model.AuthorsNames.Select(x => x.Name),
                    user.Id);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                model.Categories = this.booksService.GetBookCategories();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added book!";
            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Download(string id)
        {
            var result = await this.blobService.DownloadBlobAsync(id);
            return this.File(result.Item1, result.Item2, result.Item3);
        }
    }
}
