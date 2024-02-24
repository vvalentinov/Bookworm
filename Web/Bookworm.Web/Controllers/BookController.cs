namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.DTOs;
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

        [HttpGet]
        public IActionResult Upload()
        {
            return this.View(new UploadBookViewModel());
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(UploadBookViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var uploadBookDto = AutoMapperConfig.MapperInstance.Map<BookDto>(model);
            uploadBookDto.BookCreatorId = this.userManager.GetUserId(this.User);

            try
            {
                await this.uploadBookService.UploadBookAsync(uploadBookDto);

                this.TempData[MessageConstant.SuccessMessage] = BookUploadSuccess;

                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;

                return this.View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int bookId)
        {
            var model = await this.retrieveBooksService.GetEditBookAsync(bookId);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBookViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var editBookDto = AutoMapperConfig.MapperInstance.Map<BookDto>(model);

            try
            {
                await this.updateBookService.EditBookAsync(editBookDto, user.Id);

                this.TempData[MessageConstant.SuccessMessage] = BookEditSuccess;

                return this.RedirectToAction(nameof(this.Details), "Book", new { id = model.Id });
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;

                return this.View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookId)
        {
            string userId = this.userManager.GetUserId(this.User);

            try
            {
                await this.updateBookService.DeleteBookAsync(bookId, userId);
                this.TempData[MessageConstant.SuccessMessage] = BookDeleteSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Details), "Book", new { id = bookId });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Random()
        {
            var model = new RandomBookFormViewModel();

            return this.View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Random(RandomBookFormViewModel model)
        {
            var books = this.randomBookService.GenerateBooks(model.CategoryName, model.CountBooks);

            return this.View("GeneratedBooks", books);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(string categoryName, int page = 1)
        {
            int categoryId = this.categoriesService.GetCategoryId(categoryName);

            var model = await this.retrieveBooksService.GetBooksAsync(categoryId, page, 12);

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserBooks(int page = 1)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            BookListingViewModel books = await this.retrieveBooksService.GetUserBooksAsync(user.Id, page, 12);
            return this.View(books);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            string userId = this.userManager.GetUserId(this.User);

            var bookViewModel = await this.retrieveBooksService.GetBookDetails(id, userId);

            return this.View(bookViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var result = await this.blobService.DownloadBlobAsync(id);
            return this.File(result.Item1, result.Item2, result.Item3);
        }
    }
}
