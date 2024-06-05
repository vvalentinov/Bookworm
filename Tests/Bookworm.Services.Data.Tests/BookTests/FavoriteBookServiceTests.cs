namespace Bookworm.Services.Data.Tests.BookTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.FavoriteBookMessagesConstants;

    public class FavoriteBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public FavoriteBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task AddBookToFavoritesShouldWorkCorrectly()
        {
            var service = this.GetFavBookService();
            var favBookRepo = this.GetFavBookRepo();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";
            var bookId = 4;

            await service.AddBookToFavoritesAsync(bookId, userId);

            var favBook = await favBookRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

            Assert.NotNull(favBook);
        }

        [Fact]
        public async Task AddBookToFavoritesShouldThrowExceptionIfIdIsInvalid()
        {
            var service = this.GetFavBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.AddBookToFavoritesAsync(11, string.Empty));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task AddBookToFavoritesShouldThrowExceptionIfBookIsAlreadyPresentInFavorites()
        {
            var service = this.GetFavBookService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.AddBookToFavoritesAsync(1, userId));

            Assert.Equal(FavoriteBookIsAlreadyPresentError, exception.Message);
        }

        [Fact]
        public async Task DeleteBookFromFavoritesShouldWorkCorrectly()
        {
            var service = this.GetFavBookService();
            var favBookRepo = this.GetFavBookRepo();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";
            var bookId = 2;

            await service.DeleteBookFromFavoritesAsync(bookId, userId);

            var favBook = await favBookRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            Assert.Null(favBook);
        }

        [Fact]
        public async Task DeleteBookFromFavoritesShouldThrowExceptionIfIdIsInvalid()
        {
            var service = this.GetFavBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DeleteBookFromFavoritesAsync(11, string.Empty));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteBookFromFavoritesShouldThrowExceptionIfBookIsNotPresentInFavorites()
        {
            var service = this.GetFavBookService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DeleteBookFromFavoritesAsync(2, userId));

            Assert.Equal(FavoriteBookIsNotPresentError, exception.Message);
        }

        private EfRepository<FavoriteBook> GetFavBookRepo() => new(this.dbContext);

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private FavoriteBookService GetFavBookService() => new(this.GetFavBookRepo(), this.GetBookRepo());
    }
}
