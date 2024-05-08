namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    [Collection("Database")]
    public class UploadQuoteServiceTests
    {
        private readonly UploadQuoteService uploadQuoteService;
        private readonly IDeletableEntityRepository<Quote> quoteRepoMock;

        public UploadQuoteServiceTests(DatabaseFixture dbFixture)
        {
            this.quoteRepoMock = dbFixture.QuoteRepositoryMock.Object;
            this.uploadQuoteService = new UploadQuoteService(this.quoteRepoMock);
        }

        [Fact]
        public async Task QuoteUploadShouldWorkCorrectly()
        {
            var quoteDto = new QuoteDto
            {
                Content = "May the Force be with you",
                MovieTitle = "Star Wars",
                Type = QuoteType.MovieQuote,
            };

            await this.uploadQuoteService.UploadQuoteAsync(
                quoteDto,
                "0fc3ea28-3165-440e-947e-670c90562320");

            var quotesCount = await this.quoteRepoMock
                .AllAsNoTracking()
                .CountAsync();

            var quoteExists = await this.quoteRepoMock
                .AllAsNoTracking()
                .AnyAsync(q => q.Content == quoteDto.Content);

            Assert.True(quoteExists);
            Assert.Equal(2, quotesCount);
        }

        [Fact]
        public async Task QuoteUploadShouldThrowExceptionIfQuoteExist()
        {
            var quoteDto = new QuoteDto
            {
                Content = "Knowledge is power",
                AuthorName = "Sir Francis Bacon",
                Type = QuoteType.GeneralQuote,
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await this.uploadQuoteService.UploadQuoteAsync(
                    quoteDto,
                    "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.NotNull(exception);
            Assert.Equal(QuoteExistsError, exception.Message);
        }
    }
}
