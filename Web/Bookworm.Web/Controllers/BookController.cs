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

    public class BookController : BaseController
    {
        private readonly IBooksService booksService;
        private readonly ICategoriesService categoriesService;
        private readonly IUploadBookService uploadBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILanguagesService languagesService;
        private readonly IRandomBookService randomBookService;
        private readonly IBlobService blobService;
        private readonly IEditBookService editBookService;

        public BookController(
            IBooksService booksService,
            ICategoriesService categoriesService,
            IUploadBookService uploadBookService,
            UserManager<ApplicationUser> userManager,
            ILanguagesService languagesService,
            IRandomBookService randomBookService,
            IBlobService blobService,
            IEditBookService editBookService)
        {
            this.booksService = booksService;
            this.categoriesService = categoriesService;
            this.uploadBookService = uploadBookService;
            this.userManager = userManager;
            this.languagesService = languagesService;
            this.randomBookService = randomBookService;
            this.blobService = blobService;
            this.editBookService = editBookService;
        }

        [Authorize]
        public IActionResult Edit(string bookId)
        {
            var book = this.booksService.GetBookWithId(bookId);
            var model = new EditBookFormModel()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                PagesCount = book.PagesCount,
                PublishedYear = book.Year,
                Publisher = book.PublisherName,
                Categories = this.categoriesService.GetCategoriesAsSelectListItems(),
                Languages = this.languagesService.GetAllLanguages(),
            };
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditBookFormModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                model.Categories = this.categoriesService.GetCategoriesAsSelectListItems();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            try
            {
                await this.editBookService.EditBookAsync(
                         model.Id,
                         model.Title,
                         model.Description,
                         model.CategoryId,
                         model.LanguageId,
                         model.PagesCount,
                         model.PublishedYear,
                         model.Publisher,
                         model.AuthorsNames.Select(x => x.Name));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                model.Categories = this.categoriesService.GetCategoriesAsSelectListItems();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            return this.RedirectToAction("CurrentBook", "Book", new { id = model.Id });
        }

        public IActionResult Random()
        {
            RandomBookFormViewModel model = new()
            {
                Categories = this.randomBookService.GetCategories(),
            };

            return this.View(model);
        }

        [HttpPost]
        public IActionResult Random(RandomBookFormViewModel model)
        {
            var books = this.randomBookService.GenerateBooks(model.CategoryName, model.CountBooks);
            return this.View("GeneratedBooks", books);
        }

        public IActionResult All(string categoryName, int page = 1)
        {
            int categoryId = this.categoriesService.GetCategoryId(categoryName);

            BookListingViewModel model = this.booksService.GetBooks(categoryId, page, 12);
            return this.View(model);
        }

        [Authorize]
        public async Task<IActionResult> UserBooks(int page = 1)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            BookListingViewModel books = this.booksService.GetUserBooks(user.Id, page, 12);
            return this.View(books);
        }

        public async Task<IActionResult> CurrentBook(string id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            BookViewModel bookViewModel = this.booksService.GetBookWithId(id, user?.Id);
            return this.View(bookViewModel);
        }

        [Authorize]
        public IActionResult Upload()
        {
            return this.View(new UploadBookFormModel()
            {
                Categories = this.categoriesService.GetCategoriesAsSelectListItems(),
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
                model.Categories = this.categoriesService.GetCategoriesAsSelectListItems();
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
                    user.Id,
                    user.UserName);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                model.Categories = this.categoriesService.GetCategoriesAsSelectListItems();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added book!";
            return this.View("Views/Book/ThankYouBook.cshtml");
        }

        [Authorize]
        public async Task<IActionResult> Download(string id)
        {
            var result = await this.blobService.DownloadBlobAsync(id);
            return this.File(result.Item1, result.Item2, result.Item3);
        }
    }
}
