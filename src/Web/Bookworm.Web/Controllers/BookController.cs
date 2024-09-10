namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.Infrastructure.Filters;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;

    using static StaticKeys.ViewDataKeys;

    public class BookController : BaseController
    {
        private readonly IBlobService blobService;
        private readonly ILanguagesService languagesService;
        private readonly ICategoriesService categoriesService;
        private readonly IUploadBookService uploadBookService;
        private readonly IUpdateBookService updateBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IValidateBookService validateBookService;
        private readonly IDownloadBookService downloadBookService;
        private readonly IFavoriteBookService favoriteBookService;
        private readonly IRetrieveBooksService retrieveBooksService;

        public BookController(
            IBlobService blobService,
            ILanguagesService languagesService,
            ICategoriesService categoriesService,
            IUploadBookService uploadBookService,
            IUpdateBookService updateBookService,
            UserManager<ApplicationUser> userManager,
            IValidateBookService validateBookService,
            IDownloadBookService downloadBookService,
            IFavoriteBookService favoriteBookService,
            IRetrieveBooksService retrieveBooksService)
        {
            this.blobService = blobService;
            this.userManager = userManager;
            this.languagesService = languagesService;
            this.categoriesService = categoriesService;
            this.uploadBookService = uploadBookService;
            this.updateBookService = updateBookService;
            this.validateBookService = validateBookService;
            this.downloadBookService = downloadBookService;
            this.favoriteBookService = favoriteBookService;
            this.retrieveBooksService = retrieveBooksService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            this.ViewData[Title] = $"{nameof(this.Upload)} Book";
            this.ViewData[ControllerAction] = nameof(this.Upload);

            return this.View(new UploadBookViewModel());
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload(UploadBookViewModel model)
        {
            this.ViewData[Title] = $"{nameof(this.Upload)} Book";
            this.ViewData[ControllerAction] = nameof(this.Upload);

            if (model.BookFile == null || model.BookFile.Length == 0)
            {
                this.ModelState.AddModelError(
                    nameof(model.BookFile),
                    BookFileRequiredError);
            }

            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                this.ModelState.AddModelError(
                    nameof(model.ImageFile),
                    BookImageFileRequiredError);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var userId = this.User.GetId();

            var bookDto = model.MapToBookDto();

            var result = await this.uploadBookService.UploadBookAsync(
                bookDto,
                userId);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction("Index", "Home");
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.Upload));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            this.ViewData[ControllerAction] = nameof(this.Edit);
            this.ViewData[Title] = $"{nameof(this.Edit)} Book";

            var userId = this.User.GetId();

            var result = await this.retrieveBooksService.GetEditBookAsync(
                bookId: id,
                userId);

            if (result.IsSuccess)
            {
                return this.View(nameof(this.Upload), result.Data);
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Edit(UploadBookViewModel model)
        {
            this.ViewData[ControllerAction] = nameof(this.Edit);
            this.ViewData[Title] = $"{nameof(this.Edit)} Book";

            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(this.Upload), model);
            }

            var userId = this.User.GetId();

            var bookDto = model.MapToBookDto();

            var result = await this.updateBookService.EditBookAsync(
                bookDto,
                userId);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction(nameof(this.UserBooks), "Book");
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.UserBooks), "Book");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookId)
        {
            string userId = this.User.GetId();
            bool isUserAdmin = this.User.IsAdmin();

            var result = await this.updateBookService.DeleteBookAsync(
                bookId,
                userId,
                isUserAdmin);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction("Index", "Home");
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(
                nameof(this.Details),
                "Book",
                new { id = bookId });
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

            var result = await this.retrieveBooksService.GetRandomBooksAsync(
                    model.CountBooks,
                    model.CategoryId);

            if (result.IsSuccess)
            {
                return this.View("GeneratedBooks", result.Data);
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.Random));
        }

        [HttpGet]
        [AllowAnonymous]
        [PageValidationFilter]
        public async Task<IActionResult> All(string category, int page = 1)
        {
            this.ViewData[Title] = category;

            var result = await this.retrieveBooksService
                .GetBooksInCategoryAsync(category, page);

            if (result.IsSuccess)
            {
                return this.View(result.Data);
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.All), "Category");
        }

        [HttpGet]
        [PageValidationFilter]
        public async Task<IActionResult> Favorites(int page = 1)
        {
            var userId = this.User.GetId();

            var result = await this.retrieveBooksService
                .GetUserFavoriteBooksAsync(userId, page);

            return this.View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFromFavorites(int id)
        {
            var userId = this.User.GetId();

            var result = await this.favoriteBookService
                .DeleteBookFromFavoritesAsync(bookId: id, userId);

            if (result.IsSuccess)
            {
                return this.RedirectToAction(nameof(this.Favorites));
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.Favorites));
        }

        [HttpGet]
        [PageValidationFilter]
        public async Task<IActionResult> UserBooks(int page = 1)
        {
            var userId = this.User.GetId();

            var result = await this.retrieveBooksService
                .GetUserBooksAsync(userId, page);

            return this.View(result.Data);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var userId = this.User.GetId();
            var isAdmin = this.User.IsAdmin();

            var result = await this.retrieveBooksService.GetBookDetailsAsync(
                bookId: id,
                userId,
                isAdmin);

            if (result.IsSuccess)
            {
                return this.View(result.Data);
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var result = await this.downloadBookService.DownloadBookAsync(
                bookId: id,
                isUserAdmin: this.User.IsAdmin(),
                user.Id);

            if (result.IsSuccess)
            {
                var (stream, contentType, downloadName) = result.Data;
                return this.File(stream, contentType, downloadName);
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.Details), new { id });
        }
    }
}
