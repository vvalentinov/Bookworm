namespace Bookworm.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Books.BooksSuccessMessagesConstants;

    public class BookController : BaseController
    {
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly ICategoriesService categoriesService;
        private readonly IUploadBookService uploadBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILanguagesService languagesService;
        private readonly IRandomBookService randomBookService;
        private readonly IBlobService blobService;
        private readonly IUpdateBookService updateBookService;
        private readonly IValidateUploadedBookService validateBookService;

        public BookController(
            IRetrieveBooksService retrieveBooksService,
            ICategoriesService categoriesService,
            IUploadBookService uploadBookService,
            UserManager<ApplicationUser> userManager,
            ILanguagesService languagesService,
            IRandomBookService randomBookService,
            IBlobService blobService,
            IUpdateBookService updateBookService,
            IValidateUploadedBookService validateBookService)
        {
            this.retrieveBooksService = retrieveBooksService;
            this.categoriesService = categoriesService;
            this.uploadBookService = uploadBookService;
            this.userManager = userManager;
            this.languagesService = languagesService;
            this.randomBookService = randomBookService;
            this.blobService = blobService;
            this.updateBookService = updateBookService;
            this.validateBookService = validateBookService;
        }

        [Authorize]
        public IActionResult Upload()
        {
            UploadBookViewModel model = new UploadBookViewModel();
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(UploadBookViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            try
            {
                await this.validateBookService.ValidateUploadedBookAsync(
                    model.BookFile,
                    model.ImageFile,
                    model.Authors.Select(x => x.Name),
                    model.CategoryId,
                    model.LanguageId);
            }
            catch (InvalidOperationException exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.View(model);
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

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
                model.Authors.Select(x => x.Name),
                user.Id,
                user.UserName);

            this.TempData[MessageConstant.SuccessMessage] = BookUploadSuccess;
            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Edit(string bookId)
        {
            EditBookFormModel model = await this.retrieveBooksService.GetEditBookAsync(bookId);

            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditBookFormModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                model.Categories = this.categoriesService.GetAll<CategoryViewModel>();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            try
            {
                await this.updateBookService.EditBookAsync(
                         model.Id,
                         model.Title,
                         model.Description,
                         model.CategoryId,
                         model.LanguageId,
                         model.PagesCount,
                         model.PublishedYear,
                         model.Publisher,
                         model.Authors);
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                model.Categories = this.categoriesService.GetAll<CategoryViewModel>();
                model.Languages = this.languagesService.GetAllLanguages();
                return this.View(model);
            }

            this.TempData[MessageConstant.SuccessMessage] = "Successfully edited book!";
            return this.RedirectToAction(nameof(this.Details), "Book", new { id = model.Id });
        }

        public IActionResult Random()
        {
            RandomBookFormViewModel model = new RandomBookFormViewModel();
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Random(RandomBookFormViewModel model)
        {
            IEnumerable<BookViewModel> books = this.randomBookService.GenerateBooks(model.CategoryName, model.CountBooks);
            return this.View("GeneratedBooks", books);
        }

        public async Task<IActionResult> All(string categoryName, int page = 1)
        {
            int categoryId = this.categoriesService.GetCategoryId(categoryName);

            BookListingViewModel model = await this.retrieveBooksService.GetBooksAsync(categoryId, page, 12);
            return this.View(model);
        }

        [Authorize]
        public async Task<IActionResult> UserBooks(int page = 1)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            BookListingViewModel books = await this.retrieveBooksService.GetUserBooksAsync(user.Id, page, 12);
            return this.View(books);
        }

        public async Task<IActionResult> Details(string id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            BookViewModel bookViewModel = await this.retrieveBooksService.GetBookWithIdAsync(id, user?.Id);
            return this.View(bookViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Download(string id)
        {
            var result = await this.blobService.DownloadBlobAsync(id);
            return this.File(result.Item1, result.Item2, result.Item3);
        }
    }
}
