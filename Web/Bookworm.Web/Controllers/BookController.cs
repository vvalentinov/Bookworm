namespace Bookworm.Web.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.Infrastructure.Filters;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;
    using static Bookworm.Services.Mapping.AutoMapperConfig;

    public class BookController : BaseController
    {
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly ICategoriesService categoriesService;
        private readonly IUploadBookService uploadBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILanguagesService languagesService;
        private readonly IBlobService blobService;
        private readonly IUpdateBookService updateBookService;
        private readonly IValidateBookFilesSizesService validateBookService;
        private readonly IDownloadBookService downloadBookService;

        public BookController(
            IRetrieveBooksService retrieveBooksService,
            ICategoriesService categoriesService,
            IUploadBookService uploadBookService,
            UserManager<ApplicationUser> userManager,
            ILanguagesService languagesService,
            IBlobService blobService,
            IUpdateBookService updateBookService,
            IValidateBookFilesSizesService validateBookService,
            IDownloadBookService downloadBookService)
        {
            this.retrieveBooksService = retrieveBooksService;
            this.categoriesService = categoriesService;
            this.uploadBookService = uploadBookService;
            this.userManager = userManager;
            this.languagesService = languagesService;
            this.blobService = blobService;
            this.updateBookService = updateBookService;
            this.validateBookService = validateBookService;
            this.downloadBookService = downloadBookService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            this.ViewData["Title"] = "Upload Book";
            this.ViewData["Action"] = nameof(this.Upload);

            return this.View(new UploadBookViewModel());
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload(UploadBookViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            this.ViewData["Title"] = "Upload Book";
            this.ViewData["Action"] = nameof(this.Upload);

            var uploadBookDto = MapperInstance.Map<BookDto>(model);
            uploadBookDto.BookCreatorId = this.userManager.GetUserId(this.User);

            try
            {
                await this.uploadBookService.UploadBookAsync(uploadBookDto);
                this.TempData[SuccessMessage] = UploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int bookId)
        {
            this.ViewData["Title"] = "Edit Book";
            this.ViewData["Action"] = nameof(this.Edit);

            try
            {
                var model = await this.retrieveBooksService.GetEditBookAsync(bookId);
                return this.View(nameof(this.Upload), model);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Edit(UploadBookViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(this.Upload), model);
            }

            this.ViewData["Title"] = "Edit Book";
            this.ViewData["Action"] = nameof(this.Edit);

            var user = await this.userManager.GetUserAsync(this.User);
            var editBookDto = MapperInstance.Map<BookDto>(model);

            try
            {
                await this.updateBookService.EditBookAsync(editBookDto, user.Id);
                this.TempData[SuccessMessage] = EditSuccess;
                return this.RedirectToAction(nameof(this.UserBooks), "Book");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.View(nameof(this.Upload), model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookId)
        {
            string userId = this.userManager.GetUserId(this.User);

            try
            {
                await this.updateBookService.DeleteBookAsync(bookId, userId);
                this.TempData[SuccessMessage] = DeleteSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Details), "Book", new { id = bookId });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Random() => this.View(new RandomBookInputModel());

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Random(RandomBookInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(this.Random), model);
            }

            try
            {
                var books = await this.retrieveBooksService
                    .GetRandomBooksAsync(model.CountBooks, model.CategoryId);

                return this.View("GeneratedBooks", books);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Random));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [PageValidationFilter]
        public async Task<IActionResult> All(string category, int page = 1)
        {
            try
            {
                int categoryId = await this.categoriesService.GetCategoryIdAsync(category);
                var model = await this.retrieveBooksService.GetBooksAsync(categoryId, page);

                this.ViewData["Title"] = category;
                return this.View(model);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.All), "Category");
            }
        }

        [HttpGet]
        [PageValidationFilter]
        public async Task<IActionResult> UserBooks(int page = 1)
        {
            var userId = this.userManager.GetUserId(this.User);
            var books = await this.retrieveBooksService.GetUserBooksAsync(userId, page);

            return this.View(books);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                var model = await this.retrieveBooksService.GetBookDetails(id, userId);
                return this.View(model);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                bool isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);

                (Stream stream,
                 string contentType,
                 string downloadName) = await this.downloadBookService.DownloadBookAsync(id, user, isAdmin);

                return this.File(stream, contentType, downloadName);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Details), new { id });
            }
        }
    }
}
