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
        private readonly IDeleteBookService deleteBookService;
        private readonly IBooksService booksService;
        private readonly IApproveBookService approveBookService;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public BookController(
            IDeleteBookService deleteBookService,
            IBooksService booksService,
            IApproveBookService approveBookService,
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            this.deleteBookService = deleteBookService;
            this.booksService = booksService;
            this.approveBookService = approveBookService;
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Delete(string bookId)
        {
            try
            {
                await this.deleteBookService.DeleteBookAsync(bookId);
                this.TempData[MessageConstant.SuccessMessage] = "Successfully deleted book!";
            }
            catch (Exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("Index", "Home", new { area = " " });
        }

        public IActionResult AllBooks()
        {
            var books = this.booksService.GetUnapprovedBooks();
            return this.View(books);
        }

        public IActionResult UnapprovedBooks()
        {
            IEnumerable<BookViewModel> books = this.booksService.GetUnapprovedBooks();
            return this.View(books);
        }

        public IActionResult CurrentBook(string bookId)
        {
            var book = this.booksService.GetUnapprovedBookWithId(bookId);
            return this.View(book);
        }

        public async Task<IActionResult> ApproveBook(string bookId, string userId)
        {
            await this.approveBookService.ApproveBook(bookId, userId);
            return this.RedirectToAction(nameof(this.AllBooks), "Book");
        }
    }
}
