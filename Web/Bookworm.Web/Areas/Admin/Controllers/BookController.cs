﻿namespace Bookworm.Web.Areas.Admin.Controllers
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

        public async Task<IActionResult> UnapprovedBooks()
        {
            List<BookViewModel> books = await this.retrieveBooksService.GetUnapprovedBooksAsync();
            return this.View(books);
        }

        public async Task<IActionResult> DeletedBooks()
        {
            List<BookViewModel> books = await this.retrieveBooksService.GetDeletedBooksAsync();
            return this.View(books);
        }

        public async Task<IActionResult> ApprovedBooks()
        {
            List<BookViewModel> approvedBooks = await this.retrieveBooksService.GetApprovedBooksAsync();
            return this.View(approvedBooks);
        }

        public async Task<IActionResult> CurrentBook(string bookId)
        {
            BookViewModel book = await this.retrieveBooksService.GetBookWithIdAsync(bookId);
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
