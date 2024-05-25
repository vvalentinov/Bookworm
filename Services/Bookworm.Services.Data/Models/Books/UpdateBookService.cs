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

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.AuthorErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.LanguageErrorMessagesConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IBlobService blobService;
        private readonly IValidateBookFilesSizesService validateUploadedBookService;
        private readonly IUsersService usersService;
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IMailGunEmailSender emailSender;
        private readonly ICategoriesService categoriesService;
        private readonly ILanguagesService languagesService;
        private readonly IAuthorsService authorsService;
        private readonly IPublishersService publishersService;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IBlobService blobService,
            IValidateBookFilesSizesService validateUploadedBookService,
            IUsersService usersService,
            IRetrieveBooksService retrieveBooksService,
            IMailGunEmailSender emailSender,
            ICategoriesService categoriesService,
            ILanguagesService languagesService,
            IAuthorsService authorsService,
            IPublishersService publishersService)
        {
            this.bookRepository = bookRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
            this.usersService = usersService;
            this.retrieveBooksService = retrieveBooksService;
            this.emailSender = emailSender;
            this.categoriesService = categoriesService;
            this.languagesService = languagesService;
            this.authorsService = authorsService;
            this.publishersService = publishersService;
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
            var book = await this.bookRepository.All()
                .Include(x => x.Publisher)
                .Include(b => b.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == editBookDto.Id) ??
                throw new InvalidOperationException(BookWrongIdError);

            if (book.UserId != userId)
            {
                throw new InvalidOperationException(BookEditError);
            }

            if (!await this.categoriesService.CheckIfIdIsValidAsync(editBookDto.CategoryId))
            {
                throw new InvalidOperationException(CategoryNotFoundError);
            }

            if (!await this.languagesService.CheckIfIdIsValidAsync(editBookDto.LanguageId))
            {
                throw new InvalidOperationException(LanguageNotFoundError);
            }

            if (this.authorsService.HasDuplicates(editBookDto.Authors))
            {
                throw new InvalidOperationException(AuthorDuplicatesError);
            }

            this.validateUploadedBookService.ValidateUploadedBookFileSizes(
                isForEdit: true,
                editBookDto.BookFile,
                editBookDto.ImageFile);

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

            if (!string.IsNullOrWhiteSpace(editBookDto.Publisher))
            {
                var publisherName = editBookDto.Publisher.Trim();
                if (book.Publisher.Name != publisherName)
                {
                    var publisher = await this.publishersService.GetPublisherWithNameAsync(publisherName);
                    book.Publisher = publisher ?? new Publisher { Name = publisherName };
                }
            }

            book.Title = editBookDto.Title;
            book.Description = editBookDto.Description;
            book.CategoryId = editBookDto.CategoryId;
            book.LanguageId = editBookDto.LanguageId;
            book.PagesCount = editBookDto.PagesCount;
            book.Year = editBookDto.Year;
            book.IsApproved = false;

            book.AuthorsBooks.Clear();

            var authorsNames = editBookDto.Authors.Select(x => x.Name.Trim()).ToList();

            foreach (var authorName in authorsNames)
            {
                var author = await this.authorsService.GetAuthorWithNameAsync(authorName);

                book.AuthorsBooks.Add(author != null ?
                    new AuthorBook { Book = book, Author = author } :
                    new AuthorBook { Book = book, Author = new Author { Name = authorName } });
            }

            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(userId, BookUploadPoints);
        }
    }
}
