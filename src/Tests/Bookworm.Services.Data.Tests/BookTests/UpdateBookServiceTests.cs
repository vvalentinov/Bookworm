namespace Bookworm.Services.Data.Tests.BookTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Services.Messaging.Hubs;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class UpdateBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public UpdateBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task ApproveBookShouldWorkCorrectly()
        {
            var bookId = 6;
            var bookRepo = this.GetBookRepo();
            var userManager = this.GetUserManager();
            var service = this.GetUpdateBookService();

            await service.ApproveBookAsync(bookId);

            var book = await bookRepo.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == bookId);
            var user = await userManager.FindByIdAsync("b1a9a91f-f7b1-4459-9864-4a4fdd6077c5");

            Assert.NotNull(book);
            Assert.True(book.IsApproved);
            Assert.Equal(7, user.Points);
        }

        [Fact]
        public async Task UnapproveBookShouldWorkCorrectly()
        {
            var bookId = 4;
            var bookRepo = this.GetBookRepo();
            var userManager = this.GetUserManager();
            var service = this.GetUpdateBookService();

            await service.UnapproveBookAsync(bookId);

            var book = await bookRepo.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == bookId);
            var user = await userManager.FindByIdAsync("a84ea5dc-a89e-442f-8e53-c874675bb114");

            Assert.NotNull(book);
            Assert.False(book.IsApproved);
            Assert.Equal(0, user.Points);
        }

        [Fact]
        public async Task DeleteBookShouldWorkCorrectly()
        {
            var bookId = 10;
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";
            var bookRepo = this.GetBookRepo();
            var userManager = this.GetUserManager();
            var service = this.GetUpdateBookService();

            await service.DeleteBookAsync(bookId, userId);

            var book = await bookRepo.AllAsNoTrackingWithDeleted().FirstOrDefaultAsync(x => x.Id == bookId);
            var user = await userManager.FindByIdAsync(userId);

            Assert.NotNull(book);
            Assert.True(book.IsDeleted);
            Assert.Equal(0, user.Points);
        }

        [Fact]
        public async Task DeleteBookShouldThrowExceptionIfUserDontHavePermissionForDelete()
        {
            var service = this.GetUpdateBookService();
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DeleteBookAsync(2, userId));

            Assert.Equal(BookDeleteError, exception.Message);
        }

        [Fact]
        public async Task UndeleteBookShouldWorkCorrectly()
        {
            var bookId = 7;
            var bookRepo = this.GetBookRepo();
            var service = this.GetUpdateBookService();

            await service.UndeleteBookAsync(bookId);

            var book = await bookRepo.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == bookId);

            Assert.NotNull(book);
            Assert.False(book.IsDeleted);
        }

        [Fact]
        public async Task EditBookShouldWorkCorrectly()
        {
            var bookId = 1;
            var bookTitle = "Updated Title";
            var publisher = "Updated Publisher";
            var description = "Updated Description";
            var bookRepo = this.GetBookRepo();
            var service = this.GetUpdateBookService();

            var bookDto = new BookDto
            {
                Id = bookId,
                Year = 2012,
                Title = bookTitle,
                CategoryId = 2,
                LanguageId = 3,
                PagesCount = 12,
                Authors = new List<UploadAuthorViewModel> { new() { Name = "Author One" }, new() { Name = "Author Seven" } },
                Publisher = publisher,
                BookFile = this.GetFile(),
                ImageFile = this.GetFile(),
                Description = description,
            };

            await service.EditBookAsync(bookDto, "0fc3ea28-3165-440e-947e-670c90562320");

            var book = await bookRepo.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == bookId);

            Assert.NotNull(book);
            Assert.Equal(bookTitle, book.Title);
            Assert.Equal(description, book.Description);
        }

        [Fact]
        public async Task EditBookShouldThrowExceptionIfBookIdIsInvalid()
        {
            var service = this.GetUpdateBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.EditBookAsync(new BookDto { Id = 11 }, string.Empty));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task EditBookShouldThrowExceptionIfCurrentUserIsNotBookCreator()
        {
            var service = this.GetUpdateBookService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.EditBookAsync(new BookDto { Id = 2 }, userId));

            Assert.Equal(BookEditError, exception.Message);
        }

        private static IHubContext<NotificationHub> GetNotificationHubContext()
        {
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            var message = "ApprovedQuoteMessage";

            mockClients.Setup(clients => clients.User(It.IsAny<string>())).Returns(mockClientProxy.Object);
            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);

            mockClientProxy.Setup(x => x.SendCoreAsync(It.Is<string>(s => s == "notification"), It.Is<object[]>(o => o.Length == 1 && (string)o[0] == message), default))
                           .Returns(Task.CompletedTask);

            return mockHubContext.Object;
        }

        private IFormFile GetFile() => new Mock<IFormFile>().Object;

        private IBlobService GetBlobService()
        {
            var blobServiceMock = new Mock<IBlobService>();

            blobServiceMock
                .Setup(x => x.ReplaceBlobAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("https://some-url-here");

            return blobServiceMock.Object;
        }

        private IValidateBookService GetValidateBookService()
        {
            var validateServiceMock = new Mock<IValidateBookService>();

            validateServiceMock
                .Setup(x => x.ValidateAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int?>()));

            return validateServiceMock.Object;
        }

        private NotificationService GetNotificationService()
            => new(new EfDeletableEntityRepository<Notification>(this.dbContext));

        private IRetrieveBooksService GetRetrieveBooksService()
        {
            var bookRepo = this.GetBookRepo();

            var retrieveBookServiceMock = new Mock<IRetrieveBooksService>();

            async Task<Book> GetBookWithId(int x)
                => await bookRepo
                        .AllAsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == x);

            async Task<Book> GetDeletedBookWithId(int x)
                => await bookRepo
                        .AllAsNoTrackingWithDeleted()
                        .FirstOrDefaultAsync(b => b.Id == x && b.IsDeleted);

            retrieveBookServiceMock
                .Setup(x => x.GetBookWithIdAsync(It.IsAny<int>()))
                .Returns<int>(GetBookWithId);

            retrieveBookServiceMock
                .Setup(x => x.GetDeletedBookWithIdAsync(It.IsAny<int>()))
                .Returns<int>(GetDeletedBookWithId);

            return retrieveBookServiceMock.Object;
        }

        private UserManager<ApplicationUser> GetUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            var users = this.dbContext.Users.AsQueryable();

            var mockUserDbSet = new Mock<DbSet<ApplicationUser>>();
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => this.dbContext.Users.FirstOrDefault(u => u.Id == userId));

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => users.FirstOrDefault(u => u.Id == userId));

            return userManagerMock.Object;
        }

        private UsersService GetUsersService() => new(this.GetUserManager());

        private AuthorsService GetAuthorsService() => new(new EfRepository<Author>(this.dbContext));

        private PublishersService GetPublishersService() => new(new EfRepository<Publisher>(this.dbContext));

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private UpdateBookService GetUpdateBookService()
            => new(
                this.GetBlobService(),
                this.GetUsersService(),
                this.GetAuthorsService(),
                this.GetPublishersService(),
                this.GetValidateBookService(),
                this.GetNotificationService(),
                this.GetRetrieveBooksService(),
                GetNotificationHubContext(),
                this.GetBookRepo());
    }
}
