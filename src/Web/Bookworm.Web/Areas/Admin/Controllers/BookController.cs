namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Constants;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;

    [Authorize(Roles = AdministratorRoleName)]
    public class BookController : BaseController
    {
        private readonly IUpdateBookService updateBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public BookController(
            IRetrieveBooksService retrieveBooksService,
            IUpdateBookService updateBookService,
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.retrieveBooksService = retrieveBooksService;
            this.updateBookService = updateBookService;
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookId)
        {
            try
            {
                ApplicationUser user = await this.userManager.GetUserAsync(this.User);
                await this.updateBookService.DeleteBookAsync(bookId, user.Id);
                this.TempData[TempDataMessageConstant.SuccessMessage] = "Successfully deleted book!";
            }
            catch (Exception)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("Index", "Home", new { area = " " });
        }

        [HttpGet]
        public async Task<IActionResult> UnapprovedBooks()
        {
            var books = await this.retrieveBooksService.GetUnapprovedBooksAsync();
            return this.View(books);
        }

        [HttpGet]
        public async Task<IActionResult> DeletedBooks()
        {
            var books = await this.retrieveBooksService.GetDeletedBooksAsync();
            return this.View(books);
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedBooks()
        {
            var approvedBooks = await this.retrieveBooksService.GetApprovedBooksAsync();
            return this.View(approvedBooks);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBook(int bookId)
        {
            try
            {
                await this.updateBookService.ApproveBookAsync(bookId);
                return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
            }
            catch (Exception ex)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnapproveBook(int bookId)
        {
            await this.updateBookService.UnapproveBookAsync(bookId);
            return this.RedirectToAction(nameof(this.ApprovedBooks), "Book");
        }

        [HttpPost]
        public async Task<IActionResult> UndeleteBook(int bookId)
        {
            await this.updateBookService.UndeleteBookAsync(bookId);
            return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
        }
    }
}
