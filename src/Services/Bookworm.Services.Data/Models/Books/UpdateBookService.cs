namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data;
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
        private readonly IUnitOfWork unitOfWork;

        private readonly IDeletableEntityRepository<Book> bookRepo;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepo;
        private readonly IDeletableEntityRepository<Notification> notificationRepo;

        private readonly IBlobService blobService;
        private readonly IAuthorsService authorsService;
        private readonly IPublishersService publishersService;
        private readonly IValidateBookService validateBookService;
        private readonly IRetrieveBooksService retrieveBooksService;

        public UpdateBookService(
            IUnitOfWork unitOfWork,
            IBlobService blobService,
            IAuthorsService authorsService,
            IPublishersService publishersService,
            IValidateBookService validateBookService,
            IRetrieveBooksService retrieveBooksService)
        {
            this.unitOfWork = unitOfWork;

            this.bookRepo = this.unitOfWork.GetDeletableEntityRepository<Book>();
            this.userRepo = this.unitOfWork.GetDeletableEntityRepository<ApplicationUser>();
            this.notificationRepo = this.unitOfWork.GetDeletableEntityRepository<Notification>();

            this.blobService = blobService;
            this.authorsService = authorsService;
            this.publishersService = publishersService;
            this.validateBookService = validateBookService;
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

            this.bookRepo.Approve(book);

            var user = await this.userRepo
                .All()
                .FirstAsync(x => x.Id == book.UserId);

            user.Points += BookUploadPoints;
            this.userRepo.Update(user);

            var notificationContent = string.Format(
                ApprovedBookNotification,
                book.Title,
                BookUploadPoints);

            var notification = new Notification
            {
                UserId = book.UserId,
                Content = notificationContent,
            };

            await this.notificationRepo.AddAsync(notification);

            await this.unitOfWork.SaveChangesAsync();

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

            this.bookRepo.Unapprove(book);

            await this.ReduceUserPointsAsync(book.UserId);

            var notificationContent = string.Format(
                UnapprovedBookNotification,
                book.Title,
                BookUploadPoints);

            var notification = new Notification
            {
                UserId = book.UserId,
                Content = notificationContent,
            };

            await this.notificationRepo.AddAsync(notification);

            await this.unitOfWork.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteBookAsync(
            int bookId,
            string userId,
            bool isUserAdmin = false)
        {
            var getBookWithIdResult = await this.retrieveBooksService
                .GetBookWithIdAsync(bookId);

            if (getBookWithIdResult.IsFailure)
            {
                return OperationResult.Fail(getBookWithIdResult.ErrorMessage);
            }

            var book = getBookWithIdResult.Data;

            if (!isUserAdmin && book.UserId != userId)
            {
                return OperationResult.Fail(BookDeleteError);
            }

            this.bookRepo.Delete(book);

            await this.ReduceUserPointsAsync(book.UserId);

            await this.unitOfWork.SaveChangesAsync();

            return OperationResult.Ok(DeleteSuccess);
        }

        public async Task<OperationResult> UndeleteBookAsync(int bookId)
        {
            var getDeletedBookResult = await this.retrieveBooksService
                .GetDeletedBookWithIdAsync(bookId);

            if (getDeletedBookResult.IsFailure)
            {
                return OperationResult.Fail(getDeletedBookResult.ErrorMessage);
            }

            var book = getDeletedBookResult.Data;

            this.bookRepo.Undelete(book);
            await this.bookRepo.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> EditBookAsync(
            BookDto bookDto,
            string userId)
        {
            var book = await this.bookRepo
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

            this.bookRepo.Update(book);

            await this.ReduceUserPointsAsync(userId);

            await this.unitOfWork.SaveChangesAsync();

            return OperationResult.Ok(EditSuccess);
        }

        private async Task ReduceUserPointsAsync(string userId)
        {
            var user = await this.userRepo
                .All()
                .FirstAsync(x => x.Id == userId);

            user.Points -= BookUploadPoints;
            this.userRepo.Update(user);
        }
    }
}
