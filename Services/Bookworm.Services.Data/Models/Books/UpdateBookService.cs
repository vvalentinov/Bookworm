namespace Bookworm.Services.Data.Models.Books
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
    using Microsoft.Extensions.Configuration;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;
    using static Bookworm.Common.PointsDataConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Publisher> publishersRepository;
        private readonly IRepository<Author> authorsRepository;
        private readonly IBlobService blobService;
        private readonly IValidateUploadedBookService validateUploadedBookService;
        private readonly IUsersService usersService;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly IRetrieveBooksService retrieveBooksService;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Publisher> publishersRepository,
            IRepository<Author> authorsRepository,
            IBlobService blobService,
            IValidateUploadedBookService validateUploadedBookService,
            IUsersService usersService,
            IEmailSender emailSender,
            IConfiguration configuration,
            IRetrieveBooksService retrieveBooksService)
        {
            this.bookRepository = bookRepository;
            this.publishersRepository = publishersRepository;
            this.authorsRepository = authorsRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
            this.usersService = usersService;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.retrieveBooksService = retrieveBooksService;
        }

        public async Task ApproveBookAsync(int bookId)
        {
            var book = await this.retrieveBooksService.GetBookWithIdAsync(bookId, true);

            book.IsApproved = true;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            var bookCreator = await this.usersService.GetUserWithIdAsync(book.UserId);
            await this.usersService.IncreaseUserPointsAsync(book.UserId, BookPoints);

            var fromEmail = this.configuration.GetValue<string>("MailKitEmailSender:Email");
            var appPassword = this.configuration.GetValue<string>("MailKitEmailSender:AppPassword");
            await this.emailSender.SendEmailAsync(
                fromEmail,
                "Bookworm",
                bookCreator.Email,
                bookCreator.UserName,
                "Approved Book",
                $"<h1>Your book: {book.Title} has been approved! Congratulations!</h1>",
                appPassword);
        }

        public async Task UnapproveBookAsync(int bookId)
        {
            var book = await this.retrieveBooksService.GetBookWithIdAsync(bookId, true);
            book.IsApproved = false;
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(book.UserId, BookPoints);
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

            await this.usersService.ReduceUserPointsAsync(book.UserId, BookPoints);
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
            book.Year = editBookDto.PublishedYear;
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

            await this.usersService.ReduceUserPointsAsync(userId, BookPoints);
        }
    }
}
