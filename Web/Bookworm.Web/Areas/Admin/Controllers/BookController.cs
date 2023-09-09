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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

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
                await this.updateBookService.DeleteBookAsync(bookId);
                this.TempData[MessageConstant.SuccessMessage] = "Successfully deleted book!";
            }
            catch (Exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("Index", "Home", new { area = " " });
        }

        public IActionResult UnapprovedBooks()
        {
            IEnumerable<BookViewModel> books = this.retrieveBooksService.GetUnapprovedBooks();
            return this.View(books);
        }

        public IActionResult DeletedBooks()
        {
            IEnumerable<BookViewModel> books = this.retrieveBooksService.GetDeletedBooks();
            return this.View(books);
        }

        public IActionResult ApprovedBooks()
        {
            IEnumerable<BookViewModel> books = this.retrieveBooksService.GetApprovedBooks();
            return this.View(books);
        }

        public IActionResult CurrentBook(string bookId)
        {
            var book = this.retrieveBooksService.GetUnapprovedBookWithId(bookId);
            return this.View(book);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBook(string bookId)
        {
            await this.updateBookService.ApproveBookAsync(bookId);
            return this.RedirectToAction(nameof(this.UnapprovedBooks), "Book");
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
