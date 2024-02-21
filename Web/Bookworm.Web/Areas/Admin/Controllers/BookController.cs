namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.GlobalConstants;

    [Authorize(Roles = AdministratorRoleName)]
    public class BookController : BaseController
    {
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IUpdateBookService updateBookService;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public BookController(
            IRetrieveBooksService retrieveBooksService,
            IUpdateBookService updateBookService,
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            this.retrieveBooksService = retrieveBooksService;
            this.updateBookService = updateBookService;
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string bookId)
        {
            try
            {
                ApplicationUser user = await this.userManager.GetUserAsync(this.User);
                await this.updateBookService.DeleteBookAsync(bookId, user.Id);
                this.TempData[MessageConstant.SuccessMessage] = "Successfully deleted book!";
            }
            catch (Exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("Index", "Home", new { area = " " });
        }

        [HttpGet]
        public async Task<IActionResult> UnapprovedBooks()
        {
            List<BookViewModel> books = await this.retrieveBooksService.GetUnapprovedBooksAsync();
            return this.View(books);
        }

        [HttpGet]
        public async Task<IActionResult> DeletedBooks()
        {
            List<BookViewModel> books = await this.retrieveBooksService.GetDeletedBooksAsync();
            return this.View(books);
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedBooks()
        {
            List<BookViewModel> approvedBooks = await this.retrieveBooksService.GetApprovedBooksAsync();
            return this.View(approvedBooks);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBook(string bookId)
        {
            try
            {
                await this.updateBookService.ApproveBookAsync(bookId);
                return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnapproveBook(string bookId)
        {
            await this.updateBookService.UnapproveBookAsync(bookId);
            return this.RedirectToAction(nameof(this.ApprovedBooks), "Book");
        }

        [HttpPost]
        public async Task<IActionResult> UndeleteBook(string bookId)
        {
            await this.updateBookService.UndeleteBookAsync(bookId);
            return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
        }
    }
}
