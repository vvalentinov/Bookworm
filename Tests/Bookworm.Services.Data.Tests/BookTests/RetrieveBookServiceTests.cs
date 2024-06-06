namespace Bookworm.Services.Data.Tests.BookTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;

    public class RetrieveBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public RetrieveBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetBookDetailsShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            var book = await service.GetBookDetailsAsync(1, userId, true);

            Assert.NotNull(book);
            Assert.Equal("Book One", book.Title);
            Assert.True(book.IsUserBook);
            Assert.Equal(2, book.RatingsCount);
            Assert.Equal(3.5, book.RatingsAvg);
            Assert.Equal(2, book.Comments.Count());
        }

        [Fact]
        public async Task GetBookDetailsShouldThrowExceptionIfBookIsNotApproved()
        {
            var service = this.GetRetrieveBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetBookDetailsAsync(3, string.Empty, false));

            Assert.Equal(BookDetailsError, exception.Message);
        }

        [Fact]
        public async Task GetRandomBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var books = await service.GetRandomBooksAsync(5, 5);
            var bookTitles = books.Select(x => x.Title);

            Assert.Equal(3, books.Count());
            Assert.Contains("Book Four", bookTitles);
            Assert.Contains("Book Five", bookTitles);
            Assert.Contains("Book Ten", bookTitles);
        }

        [Fact]
        public async Task GetRandomBooksShouldThrowExceptionIfCategoryIdIsInvalid()
        {
            var service = this.GetRetrieveBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetRandomBooksAsync(5, 7));

            Assert.Equal(CategoryNotFoundError, exception.Message);
        }

        [Fact]
        public async Task GetEditBookShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";
            var bookId = 1;

            var book = await service.GetEditBookAsync(bookId, userId);

            Assert.NotNull(book);
            Assert.Equal(1, book.Id);
        }

        [Fact]
        public async Task GetEditBookShouldThrowExceptionIfBookIdIsInvalid()
        {
            var service = this.GetRetrieveBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetEditBookAsync(11, string.Empty));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetEditBookShouldThrowExceptionIfCurrUserIsNotBookCreator()
        {
            var service = this.GetRetrieveBookService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetEditBookAsync(1, userId));

            Assert.Equal(BookEditError, exception.Message);
        }

        [Fact]
        public async Task GetBooksInCategoryShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var model = await service.GetBooksInCategoryAsync("Category Five", 1);
            var books = model.Books.ToList();

            Assert.Equal(3, model.Books.Count());
            Assert.Equal("Book Ten", books[0].Title);
            Assert.Equal("Book Five", books[1].Title);
            Assert.Equal("Book Four", books[2].Title);
        }

        [Fact]
        public async Task GetUserFavoriteBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var model = await service.GetUserFavoriteBooksAsync(userId, 1);
            var books = model.Books.ToList();

            Assert.Equal(2, model.RecordsCount);
            Assert.Equal(4, books[0].Id);
            Assert.Equal(1, books[1].Id);
        }

        [Fact]
        public async Task GetUserBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var model = await service.GetUserBooksAsync(userId, 1);
            var books = model.Books.ToList();

            Assert.Equal(2, model.RecordsCount);
            Assert.Equal(10, books[0].Id);
            Assert.Equal(2, books[1].Id);
        }

        [Fact]
        public async Task GetPopularBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var result = await service.GetPopularBooksAsync();
            var books = result.ToList();

            Assert.Equal(10, books[0].Id);
            Assert.Equal(5, books[1].Id);
        }

        [Fact]
        public async Task GetRecentBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var result = await service.GetRecentBooksAsync();
            var books = result.ToList();

            Assert.Equal(10, books[0].Id);
            Assert.Equal(5, books[1].Id);
            Assert.Equal(4, books[2].Id);
            Assert.Equal(2, books[3].Id);
            Assert.Equal(1, books[4].Id);
        }

        [Fact]
        public async Task GetUnapprovedBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var result = await service.GetUnapprovedBooksAsync();
            var books = result.ToList();

            Assert.Equal(3, books[0].Id);
            Assert.Equal(6, books[1].Id);
            Assert.Equal(9, books[2].Id);
        }

        [Fact]
        public async Task GetUnapprovedBooksCountShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var count = await service.GetUnapprovedBooksCountAsync();

            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetApprovedBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var result = await service.GetApprovedBooksAsync();
            var books = result.ToList();

            Assert.Equal(1, books[0].Id);
            Assert.Equal(2, books[1].Id);
            Assert.Equal(4, books[2].Id);
            Assert.Equal(5, books[3].Id);
            Assert.Equal(10, books[4].Id);
        }

        [Fact]
        public async Task GetDeletedBooksShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var result = await service.GetDeletedBooksAsync();
            var books = result.ToList();

            Assert.Equal(7, books[0].Id);
            Assert.Equal(8, books[1].Id);
        }

        [Fact]
        public async Task GetBookWithIdShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var book = await service.GetBookWithIdAsync(1);

            Assert.NotNull(book);
            Assert.Equal(1, book.Id);
        }

        [Fact]
        public async Task GetBookWithIdShouldThrowExceptionIfIdIsInvalid()
        {
            var service = this.GetRetrieveBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetBookWithIdAsync(11));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetDeletedBookWithIdShouldWorkCorrectly()
        {
            var service = this.GetRetrieveBookService();

            var book = await service.GetDeletedBookWithIdAsync(8);

            Assert.NotNull(book);
            Assert.Equal(8, book.Id);
        }

        [Fact]
        public async Task GetDeletedBookWithIdShouldThrowExceptionIfBookIsNotDeleted()
        {
            var service = this.GetRetrieveBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetDeletedBookWithIdAsync(1));

            Assert.Equal(BookWrongIdError, exception.Message);
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

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private RatingsService GetRatingsService() => new(this.GetBookRepo());

        private CategoriesService GetCategoriesService() => new(new EfRepository<Category>(this.dbContext));

        private RetrieveBooksService GetRetrieveBookService()
            => new(this.GetUsersService(),
                this.GetRatingsService(),
                this.GetCategoriesService(),
                new EfRepository<Comment>(this.dbContext),
                new EfDeletableEntityRepository<Book>(this.dbContext),
                new EfRepository<FavoriteBook>(this.dbContext));
    }
}
