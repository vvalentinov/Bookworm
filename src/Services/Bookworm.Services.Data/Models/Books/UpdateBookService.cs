namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.NotificationConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IBlobService blobService;
        private readonly IUsersService usersService;
        private readonly IAuthorsService authorsService;
        private readonly IPublishersService publishersService;
        private readonly IValidateBookService validateBookService;
        private readonly INotificationService notificationService;
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<Notification> notificationRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UpdateBookService(
            IBlobService blobService,
            IUsersService usersService,
            IAuthorsService authorsService,
            IPublishersService publishersService,
            IValidateBookService validateBookService,
            INotificationService notificationService,
            IRetrieveBooksService retrieveBooksService,
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<Notification> notificationRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.blobService = blobService;
            this.usersService = usersService;
            this.authorsService = authorsService;
            this.bookRepository = bookRepository;
            this.notificationRepository = notificationRepository;
            this.userRepository = userRepository;
            this.publishersService = publishersService;
            this.validateBookService = validateBookService;
            this.notificationService = notificationService;
            this.retrieveBooksService = retrieveBooksService;
        }

        public async Task<OperationResult> ApproveBookAsync(int bookId)
        {
            var getBookWithIdResult = await this.retrieveBooksService
                .GetBookWithIdAsync(bookId);

            if (getBookWithIdResult.IsFailure)
            {
                return OperationResult.Fail(getBookWithIdResult.ErrorMessage);
            }

            var book = getBookWithIdResult.Data;

            book.IsApproved = true;
            this.bookRepository.Update(book);
            //await this.bookRepository.SaveChangesAsync();

            var user = await this.userRepository
                .All()
                .FirstAsync(x => x.Id == book.UserId);

            user.Points += BookUploadPoints;
            this.userRepository.Update(user);

            //await this.usersService.IncreaseUserPointsAsync(
            //    book.UserId,
            //    BookUploadPoints);

            var notificationContent = string.Format(
                ApprovedBookNotification,
                book.Title,
                BookUploadPoints);

            var notification = new Notification
            {
                UserId = book.UserId,
                Content = notificationContent,
            };

            await this.notificationRepository.AddAsync(notification);

            //await this.notificationService.AddNotificationAsync(
            //    notificationContent,
            //    book.UserId);

            await this.bookRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UnapproveBookAsync(int bookId)
        {
            var getBookWithIdResult = await this.retrieveBooksService
                .GetBookWithIdAsync(bookId);

            if (getBookWithIdResult.IsFailure)
            {
                return OperationResult.Fail(getBookWithIdResult.ErrorMessage);
            }

            var book = getBookWithIdResult.Data;

            book.IsApproved = false;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(
                book.UserId,
                BookUploadPoints);

            var notificationContent = string.Format(
                UnapprovedBookNotification,
                book.Title,
                BookUploadPoints);

            await this.notificationService.AddNotificationAsync(
                notificationContent,
                book.UserId);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteBookAsync(int bookId, string userId)
        {
            var getBookWithIdResult = await this.retrieveBooksService
                .GetBookWithIdAsync(bookId);

            if (getBookWithIdResult.IsFailure)
            {
                return OperationResult.Fail(getBookWithIdResult.ErrorMessage);
            }

            var book = getBookWithIdResult.Data;

            bool isUserAdmin = await this.usersService.IsUserAdminAsync(userId);

            if (!isUserAdmin && book.UserId != userId)
            {
                return OperationResult.Fail(BookDeleteError);
            }

            book.IsApproved = false;
            this.bookRepository.Delete(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(
                book.UserId,
                BookUploadPoints);

            return OperationResult.Ok(DeleteSuccess);
        }

        public async Task<OperationResult> UndeleteBookAsync(int bookId)
        {
            var getDeletedBookResult = await this.retrieveBooksService
                .GetDeletedBookWithIdAsync(bookId);

            if (!getDeletedBookResult.IsSuccess)
            {
                return OperationResult.Fail(getDeletedBookResult.ErrorMessage);
            }

            var book = getDeletedBookResult.Data;

            this.bookRepository.Undelete(book);
            await this.bookRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> EditBookAsync(
            BookDto bookDto,
            string userId)
        {
            var book = await this.bookRepository
                .All()
                .Include(x => x.Publisher)
                .Include(b => b.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == bookDto.Id);

            if (book == null)
            {
                return OperationResult.Fail(BookWrongIdError);
            }

            if (book.UserId != userId)
            {
                return OperationResult.Fail(BookEditError);
            }

            await this.validateBookService.ValidateAsync(
                bookDto.Title,
                bookDto.LanguageId,
                bookDto.CategoryId,
                book.Id);

            if (bookDto.BookFile != null)
            {
                string bookBlobName = book.FileUrl[book.FileUrl.IndexOf("Books")..];
                book.FileUrl = await this.blobService.ReplaceBlobAsync(
                    bookDto.BookFile,
                    bookBlobName,
                    BookFileUploadPath);
            }

            if (bookDto.ImageFile != null)
            {
                string imageBlobName = book.ImageUrl[book.ImageUrl.IndexOf("BooksImages")..];

                book.ImageUrl = await this.blobService.ReplaceBlobAsync(
                    bookDto.ImageFile,
                    imageBlobName,
                    BookImageFileUploadPath);
            }

            if (!string.IsNullOrWhiteSpace(bookDto.Publisher))
            {
                var publisherName = bookDto.Publisher.Trim();

                if (book.Publisher.Name != publisherName)
                {
                    var result = await this.publishersService
                        .GetPublisherWithNameAsync(publisherName);

                    book.Publisher = result.Data ?? new Publisher { Name = publisherName };
                }
            }

            book.IsApproved = false;
            book.Year = bookDto.Year;
            book.Title = bookDto.Title;
            book.PagesCount = bookDto.PagesCount;
            book.CategoryId = bookDto.CategoryId;
            book.LanguageId = bookDto.LanguageId;
            book.Description = bookDto.Description;

            book.AuthorsBooks.Clear();

            var authorsNames = bookDto.Authors.Select(x => x.Name.Trim()).ToList();

            foreach (var authorName in authorsNames)
            {
                var result = await this.authorsService
                    .GetAuthorWithNameAsync(authorName);

                book.AuthorsBooks.Add(result.Data != null ?
                    new AuthorBook { AuthorId = result.Data.Id } :
                    new AuthorBook { Author = new Author { Name = authorName } });
            }

            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(
                userId,
                BookUploadPoints);

            return OperationResult.Ok(EditSuccess);
        }
    }
}
