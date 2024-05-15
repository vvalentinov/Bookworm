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
            var quoteLikeRepo = this.GetQuoteLikeRepo();
            var manageQuoteLikesService = this.GetManageQuoteLikesService();

            var quoteLikesCount = await manageQuoteLikesService
                .LikeAsync(10, "0fc3ea28-3165-440e-947e-670c90562320");

            var quoteLike = await quoteLikeRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(
                    ql => ql.UserId == "0fc3ea28-3165-440e-947e-670c90562320" && ql.QuoteId == 10);

            Assert.Equal(1, quoteLikesCount);
            Assert.NotNull(quoteLike);
        }

        [Fact]
        public async Task LikeQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var manageQuoteLikesService = this.GetManageQuoteLikesService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await manageQuoteLikesService.LikeAsync(
                    100, "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task LikeQuoteShouldThrowExceptionIfUserIsQuoteCreator()
        {
            var manageQuoteLikesService = this.GetManageQuoteLikesService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await manageQuoteLikesService.LikeAsync(
                    10, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));

            Assert.Equal("User cannot like or unlike his or her quotes!", exception.Message);
        }

        [Fact]
        public async Task UnlikeQuoteShouldWorkCorrectly()
        {
            var quoteLikeRepo = this.GetQuoteLikeRepo();
            var manageQuoteLikesService = this.GetManageQuoteLikesService();

            var quoteLikes = await manageQuoteLikesService
                .UnlikeAsync(7, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var quoteLike = await quoteLikeRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(
                    ql => ql.QuoteId == 8 && ql.UserId == "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            Assert.Equal(0, quoteLikes);
            Assert.Null(quoteLike);
        }

        private EfRepository<QuoteLike> GetQuoteLikeRepo()
            => new (this.dbContext);

        private EfDeletableEntityRepository<Quote> GetQuoteRepo()
            => new (this.dbContext);

        private ManageQuoteLikesService GetManageQuoteLikesService()
            => new (this.GetQuoteLikeRepo(), this.GetQuoteRepo());
    }
}
