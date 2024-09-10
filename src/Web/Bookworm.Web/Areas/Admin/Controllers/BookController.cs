namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;

    [Authorize(Roles = AdministratorRoleName)]
    public class BookController : BaseController
    {
        private readonly IUpdateBookService updateBookService;
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public BookController(
            IUpdateBookService updateBookService,
            IRetrieveBooksService retrieveBooksService,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
            this.updateBookService = updateBookService;
            this.retrieveBooksService = retrieveBooksService;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookId)
        {
            var userId = this.User.GetId();

            var result = await this.updateBookService.DeleteBookAsync(
                bookId,
                userId,
                isUserAdmin: true);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
            }
            else
            {
                this.TempData[ErrorMessage] = result.ErrorMessage;
            }

            return this.RedirectToAction(
                "Index",
                "Home",
                new { area = " " });
        }

        [HttpGet]
        public async Task<IActionResult> UnapprovedBooks()
        {
            var result = await this.retrieveBooksService
                .GetUnapprovedBooksAsync();

            return this.View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> DeletedBooks()
        {
            var result = await this.retrieveBooksService
                .GetDeletedBooksAsync();

            return this.View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedBooks()
        {
            var result = await this.retrieveBooksService
                .GetApprovedBooksAsync();

            return this.View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBook(int bookId)
        {
            // TODO: send notification await this.notificationHub.Clients.User(book.UserId).SendAsync("notify", ApprovedBookMessage);
            var result = await this.updateBookService
                .ApproveBookAsync(bookId);

            if (result.IsSuccess)
            {
                return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
        }

        [HttpPost]
        public async Task<IActionResult> UnapproveBook(int bookId)
        {
            // await this.notificationHub.Clients.User(book.UserId).SendAsync("notify", UnapprovedBookMessage);
            var result = await this.updateBookService
                .UnapproveBookAsync(bookId);

            if (result.IsFailure)
            {
                this.TempData[ErrorMessage] = result.ErrorMessage;
            }

            return this.RedirectToAction(nameof(this.ApprovedBooks), "Book");
        }

        [HttpPost]
        public async Task<IActionResult> UndeleteBook(int bookId)
        {
            var result = await this.updateBookService
                .UndeleteBookAsync(bookId);

            if (result.IsFailure)
            {
                this.TempData[ErrorMessage] = result.ErrorMessage;
            }

            return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
        }
    }
}
