namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class UploadQuoteServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public UploadQuoteServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task QuoteUploadShouldWorkCorrectly()
        {
            var quoteRepo = this.GetQuoteRepo();
            var uploadQuoteService = this.GetUploadQuoteService();

            var quoteDto = new QuoteDto
            {
                Content = "May the Force be with you",
                MovieTitle = "Star Wars",
                Type = QuoteType.MovieQuote,
            };

            await uploadQuoteService.UploadQuoteAsync(
                quoteDto,
                "0fc3ea28-3165-440e-947e-670c90562320");

            var quote = await quoteRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(q => q.Content == quoteDto.Content);

            Assert.NotNull(quote);
            Assert.Equal("0fc3ea28-3165-440e-947e-670c90562320", quote.UserId);
        }

        [Fact]
        public async Task QuoteUploadShouldThrowExceptionIfQuoteExist()
        {
            var uploadQuoteService = this.GetUploadQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await uploadQuoteService.UploadQuoteAsync(
                    new QuoteDto
                    {
                        Content = "Knowledge is power",
                        AuthorName = "Sir Francis Bacon",
                        Type = QuoteType.GeneralQuote,
                    }, "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.NotNull(exception);
            Assert.Equal(QuoteExistsError, exception.Message);
        }

        [Fact]
        public async Task QuoteUploadShouldThrowExceptionIfTypeIsInvalid()
        {
            var uploadQuoteService = this.GetUploadQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await uploadQuoteService.UploadQuoteAsync(
                    new QuoteDto
                    {
                        Content = "Some Content Here",
                        AuthorName = "Some Author Name Here",
                        Type = QuoteType.None,
                    }, "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.NotNull(exception);
            Assert.Equal(QuoteInvalidTypeError, exception.Message);
        }

        private EfDeletableEntityRepository<Quote> GetQuoteRepo() => new (this.dbContext);

        private UploadQuoteService GetUploadQuoteService() => new (this.GetQuoteRepo());
    }
}
