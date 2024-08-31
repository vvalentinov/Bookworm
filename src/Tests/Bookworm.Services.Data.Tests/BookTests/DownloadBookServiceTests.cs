namespace Bookworm.Services.Data.Tests.BookTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Contracts;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.UserErrorMessagesConstants;

    public class DownloadBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public DownloadBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task DownloadShouldThrowExcIfUserIsNotAdminAndHasReachedDailyDownloadsCount()
        {
            var userManager = this.GetUserManager();
            var service = this.GetDownloadBookService();
            var userService = this.GetUsersService();

            var userId = "b1a9a91f-f7b1-4459-9864-4a4fdd6077c5";

            var user = await userManager.FindByIdAsync(userId);
            var userMaxDailyDownloadsCount = userService.GetUserDailyMaxDownloadsCount(user.Points);

            string errMsg = string.Format(UserDailyCountError, userMaxDailyDownloadsCount);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DownloadBookAsync(1, user));

            Assert.Equal(errMsg, exception.Message);
        }

        [Fact]
        public async Task DownloadShouldThrowExcBookIdIsInvalid()
        {
            var userManager = this.GetUserManager();
            var service = this.GetDownloadBookService();
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";
            var user = await userManager.FindByIdAsync(userId);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DownloadBookAsync(11, user));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DownloadShouldThrowExceptionIfBookIsNotApproved()
        {
            var userManager = this.GetUserManager();
            var service = this.GetDownloadBookService();
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";
            var user = await userManager.FindByIdAsync(userId);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DownloadBookAsync(6, user));

            Assert.Equal(BookNotApprovedError, exception.Message);
        }

        [Fact]
        public async Task DownloadBookShouldWorkCorrectly()
        {
            var bookRepo = this.GetBookRepo();
            var userManager = this.GetUserManager();
            var service = this.GetDownloadBookService();
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";
            var user = await userManager.FindByIdAsync(userId);

            await service.DownloadBookAsync(1, user);

            var book = await bookRepo.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == 1);

            Assert.NotNull(book);
            Assert.Equal(3, book.DownloadsCount);
            Assert.Equal(2, user.DailyDownloadsCount);
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

        private IBlobService GetBlobService()
        {
            var blobServiceMock = new Mock<IBlobService>();

            blobServiceMock.Setup(x => x.DownloadBlobAsync(It.IsAny<string>()));

            return blobServiceMock.Object;
        }

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private DownloadBookService GetDownloadBookService()
            => new(
                this.GetBlobService(),
                this.GetUsersService(),
                this.GetBookRepo());
    }
}
