namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class ManageQuoteLikesServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public ManageQuoteLikesServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task LikeQuoteShouldWorkCorrectly()
        {
            int quoteId = 2;
            string userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var quoteLikesCount = await this.GetManageQuoteLikesService()
                .LikeAsync(quoteId, userId);

            var quoteLike = this.GetQuoteLikeRepo().AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.UserId == userId && ql.QuoteId == quoteId);

            Assert.Equal(1, quoteLikesCount);
            Assert.NotNull(quoteLike);
        }

        [Fact]
        public async Task LikeQuoteShouldWorkCorrectlyWhenQuoteIsAlreadyLiked()
        {
            var quoteLikesCount = await this.GetManageQuoteLikesService()
                .LikeAsync(10, "0fc3ea28-3165-440e-947e-670c90562320");

            Assert.Equal(1, quoteLikesCount);
        }

        [Fact]
        public async Task LikeQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await this.GetManageQuoteLikesService()
                    .LikeAsync(100, string.Empty));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task LikeQuoteShouldThrowExceptionIfUserIsQuoteCreator()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await this.GetManageQuoteLikesService()
                    .LikeAsync(10, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));

            Assert.Equal("User cannot like or unlike his or her quotes!", exception.Message);
        }

        [Fact]
        public async Task UnlikeQuoteShouldWorkCorrectly()
        {
            var quoteId = 7;
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var quoteLikes = await this.GetManageQuoteLikesService()
                .UnlikeAsync(quoteId, userId);

            var quoteLike = await this.GetQuoteLikeRepo()
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            Assert.Equal(1, quoteLikes);
            Assert.Null(quoteLike);
        }

        [Fact]
        public async Task UnlikeQuoteShouldThrowExceptionIfUserIsCreator()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await this.GetManageQuoteLikesService()
                    .UnlikeAsync(10, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));

            Assert.Equal("User cannot like or unlike his or her quotes!", exception.Message);
        }

        private EfRepository<QuoteLike> GetQuoteLikeRepo()
            => new EfRepository<QuoteLike>(this.dbContext);

        private EfDeletableEntityRepository<Quote> GetQuoteRepo()
            => new EfDeletableEntityRepository<Quote>(this.dbContext);

        private ManageQuoteLikesService GetManageQuoteLikesService()
            => new ManageQuoteLikesService(this.GetQuoteLikeRepo(), this.GetQuoteRepo());
    }
}
