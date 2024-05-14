namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    [Collection("Database")]
    public class ManageQuoteLikesServiceTests
    {
        private readonly IRepository<QuoteLike> quoteLikeRepository;
        private readonly ManageQuoteLikesService manageQuoteLikesService;
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public ManageQuoteLikesServiceTests(DbContextFixture dbContextFixture)
        {
            this.quoteLikeRepository = new EfRepository<QuoteLike>(dbContextFixture.DbContext);
            this.quoteRepository = new EfDeletableEntityRepository<Quote>(dbContextFixture.DbContext);
            this.manageQuoteLikesService = new ManageQuoteLikesService(this.quoteLikeRepository, this.quoteRepository);
        }

        [Fact]
        public async Task LikeQuoteShouldWorkCorrectly()
        {
            var quoteLikesCount = await this.manageQuoteLikesService
                .LikeAsync(10, "0fc3ea28-3165-440e-947e-670c90562320");

            var quoteLike = await this.quoteLikeRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(
                    ql => ql.UserId == "0fc3ea28-3165-440e-947e-670c90562320" && ql.QuoteId == 10);

            Assert.Equal(1, quoteLikesCount);
            Assert.NotNull(quoteLike);
        }

        [Fact]
        public async Task LikeQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await this.manageQuoteLikesService.LikeAsync(
                    100, "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task LikeQuoteShouldThrowExceptionIfUserIsQuoteCreator()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await this.manageQuoteLikesService.LikeAsync(
                    10, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));

            Assert.Equal("User cannot like or unlike his or her quotes!", exception.Message);
        }

        [Fact]
        public async Task UnlikeQuoteShouldWorkCorrectly()
        {
            var quoteLikes = await this.manageQuoteLikesService
                .UnlikeAsync(7, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var quoteLike = await this.quoteLikeRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(
                    ql => ql.QuoteId == 8 && ql.UserId == "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            Assert.Equal(0, quoteLikes);
            Assert.Null(quoteLike);
        }
    }
}
