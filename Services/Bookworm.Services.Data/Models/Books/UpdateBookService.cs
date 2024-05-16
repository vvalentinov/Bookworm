﻿namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Publisher> publishersRepository;
        private readonly IRepository<Author> authorsRepository;
        private readonly IBlobService blobService;
        private readonly IValidateUploadedBookService validateUploadedBookService;
        private readonly IUsersService usersService;
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IMailGunEmailSender emailSender;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Publisher> publishersRepository,
            IRepository<Author> authorsRepository,
            IBlobService blobService,
            IValidateUploadedBookService validateUploadedBookService,
            IUsersService usersService,
            IRetrieveBooksService retrieveBooksService,
            IMailGunEmailSender emailSender)
        {
            this.bookRepository = bookRepository;
            this.publishersRepository = publishersRepository;
            this.authorsRepository = authorsRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
            this.usersService = usersService;
            this.retrieveBooksService = retrieveBooksService;
            this.emailSender = emailSender;
        }

        public async Task ApproveBookAsync(int bookId)
        {
            var book = await this.retrieveBooksService.GetBookWithIdAsync(bookId, true);

            book.IsApproved = true;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            var bookCreator = await this.usersService.GetUserWithIdAsync(book.UserId);
            await this.usersService.IncreaseUserPointsAsync(book.UserId, BookUploadPoints);

            await this.emailSender.SendEmailAsync(
                bookCreator.Email,
                "Approved Book",
                "<h1>Your book has been approved!</h1>");
        }

        public async Task UnapproveBookAsync(int bookId)
        {
            var book = await this.retrieveBooksService.GetBookWithIdAsync(bookId, true);
            book.IsApproved = false;
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(book.UserId, BookUploadPoints);
        }

        public async Task DeleteBookAsync(int bookId, string userId)
        {
            var book = await this.retrieveBooksService.GetBookWithIdAsync(bookId, true);

            bool isUserAdmin = await this.usersService.IsUserAdminAsync(userId);

            if (book.UserId != userId && !isUserAdmin)
            {
                throw new InvalidOperationException(BookDeleteError);
            }

            book.IsApproved = false;
            this.bookRepository.Delete(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(book.UserId, BookUploadPoints);
        }

        public async Task UndeleteBookAsync(int bookId)
        {
            var book = await this.retrieveBooksService.GetDeletedBookWithIdAsync(bookId, true);
            this.bookRepository.Undelete(book);
            await this.bookRepository.SaveChangesAsync();
        }

        public async Task EditBookAsync(BookDto editBookDto, string userId)
        {
            var book = await this.bookRepository
                .All()
                .Include(x => x.Publisher)
                .Include(b => b.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == editBookDto.Id) ??
                throw new InvalidOperationException(BookWrongIdError);

            if (book.UserId != userId)
            {
                throw new InvalidOperationException(BookEditError);
            }

            await this.validateUploadedBookService.ValidateUploadedBookAsync(
                true,
                editBookDto.CategoryId,
                editBookDto.LanguageId,
                editBookDto.BookFile,
                editBookDto.ImageFile,
                editBookDto.Authors);

            if (editBookDto.BookFile != null)
            {
                string bookBlobName = book.FileUrl[book.FileUrl.IndexOf("Books") ..];
                book.FileUrl = await this.blobService.ReplaceBlobAsync(
                    editBookDto.BookFile,
                    bookBlobName,
                    BookFileUploadPath);
            }

            if (editBookDto.ImageFile != null)
            {
                string imageBlobName = book.ImageUrl[book.ImageUrl.IndexOf("BooksImages") ..];
                book.ImageUrl = await this.blobService.ReplaceBlobAsync(
                    editBookDto.ImageFile,
                    imageBlobName,
                    BookImageFileUploadPath);
            }

            var publisherName = editBookDto.Publisher.Trim();

            if (editBookDto.Publisher != null && book.Publisher.Name != publisherName)
            {
                var publisher = await this.publishersRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(p => p.Name == publisherName);

                book.Publisher = publisher ?? new Publisher { Name = publisherName };
            }

            book.Title = editBookDto.Title;
            book.Description = editBookDto.Description;
            book.CategoryId = editBookDto.CategoryId;
            book.LanguageId = editBookDto.LanguageId;
            book.PagesCount = editBookDto.PagesCount;
            book.Year = editBookDto.Year;
            book.IsApproved = false;

            var authorsNames = editBookDto.Authors.Select(x => x.Name.Trim()).ToList();

            book.AuthorsBooks.Clear();

            foreach (var name in authorsNames)
            {
                var author = await this.authorsRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(a => a.Name == name);

                if (author == null)
                {
                    book.AuthorsBooks.Add(new AuthorBook
                    {
                        Author = new Author { Name = name },
                        Book = book,
                    });
                }
                else
                {
                    book.AuthorsBooks.Add(new AuthorBook
                    {
                        Author = author,
                        Book = book,
                    });
                }
            }

            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(userId, BookUploadPoints);
        }
    }
}
