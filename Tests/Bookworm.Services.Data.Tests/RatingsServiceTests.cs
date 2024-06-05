namespace Bookworm.Services.Data.Tests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.RatingErrorMessagesConstants;

    public class RatingsServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public RatingsServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetAvegareRatingShouldWorkCorrectly()
        {
            var service = this.GetRatingsService();

            var avgRating = await service.GetAverageRatingAsync(1);

            Assert.Equal(3.5, avgRating);
        }

        [Fact]
        public async Task GetAvegareRatingShouldReturnZeroIfBookHasNoRatings()
        {
            var service = this.GetRatingsService();

            var avgRating = await service.GetAverageRatingAsync(2);

            Assert.Equal(0, avgRating);
        }

        [Fact]
        public async Task GetAvegareRatingShouldThrowExceptionIfBookIdIsInvalid()
        {
            var service = this.GetRatingsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetAverageRatingAsync(11));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetUserRatingShouldWorkCorrectly()
        {
            var service = this.GetRatingsService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var rating = await service.GetUserRatingAsync(1, userId);

            Assert.Equal(2, rating);
        }

        [Fact]
        public async Task GetUserRatingShouldReturnZeroIfUserHasNotRatedBook()
        {
            var service = this.GetRatingsService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var rating = await service.GetUserRatingAsync(4, userId);

            Assert.Equal(0, rating);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(11)]
        public async Task GetUserRatingShouldThrowExceptionIfBookIdIsInvalid(int id)
        {
            var service = this.GetRatingsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetUserRatingAsync(id, string.Empty));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetRatingsCountShouldWorkCorrectly()
        {
            var service = this.GetRatingsService();

            var count = await service.GetRatingsCountAsync(1);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetRatingsCountShouldBeZeroIfNoRatingsArePresentForBook()
        {
            var service = this.GetRatingsService();

            var count = await service.GetRatingsCountAsync(4);

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetRatingsShouldThrowExceptionIfBookIdIsInvalid()
        {
            var service = this.GetRatingsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetRatingsCountAsync(11));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task SetRatingShouldWorkCorrectly()
        {
            var ratingRepo = this.GetRatingRepo();
            var service = this.GetRatingsService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            await service.SetRatingAsync(bookId: 2, userId, value: 4);

            var ratingsCount = await service.GetRatingsCountAsync(2);
            var rating = await ratingRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.BookId == 2 && x.UserId == userId);

            Assert.NotNull(rating);
            Assert.Equal(1, ratingsCount);
        }

        [Fact]
        public async Task SetRatingShouldChangeAlreadyExistingRating()
        {
            var ratingRepo = this.GetRatingRepo();
            var service = this.GetRatingsService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            await service.SetRatingAsync(bookId: 5, userId, value: 4);

            var rating = await ratingRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.BookId == 5 && x.UserId == userId);

            Assert.NotNull(rating);
            Assert.Equal(4, rating.Value);
        }

        [Fact]
        public async Task SetRatingShouldThrowExceptionIfBookIdIsInvalid()
        {
            var service = this.GetRatingsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.SetRatingAsync(0, string.Empty, 3));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task SetRatingShouldThrowExceptionIfUserTriesToRateHisOwnBook()
        {
            var service = this.GetRatingsService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.SetRatingAsync(1, userId, 3));

            Assert.Equal(RateBookError, exception.Message);
        }

        private EfRepository<Rating> GetRatingRepo() => new(this.dbContext);

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private RatingsService GetRatingsService() => new(this.GetBookRepo());
    }
}
