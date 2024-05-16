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
    using static Bookworm.Common.Enums.QuoteType;

    public class UploadQuoteServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public UploadQuoteServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Theory]
        [InlineData(MovieQuote)]
        [InlineData(BookQuote)]
        [InlineData(GeneralQuote)]
        public async Task QuoteUploadShouldWorkCorrectly(QuoteType type)
        {
            string userId = "0fc3ea28-3165-440e-947e-670c90562320";

            var quoteDto = new QuoteDto
            {
                Type = type,
                Content = Guid.NewGuid().ToString(),
            };

            switch (type)
            {
                case BookQuote:
                    quoteDto.AuthorName = Guid.NewGuid().ToString();
                    quoteDto.BookTitle = Guid.NewGuid().ToString();
                    break;
                case MovieQuote:
                    quoteDto.MovieTitle = Guid.NewGuid().ToString();
                    break;
                case GeneralQuote:
                    quoteDto.AuthorName = Guid.NewGuid().ToString();
                    break;
            }

            await this.GetUploadQuoteService().UploadQuoteAsync(quoteDto, userId);

            var quote = await this.GetQuoteRepo().AllAsNoTracking()
                .FirstOrDefaultAsync(q => q.Content == quoteDto.Content);

            Assert.NotNull(quote);
            Assert.Equal(userId, quote.UserId);
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
                        Type = GeneralQuote,
                    }, string.Empty));

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
                        Type = None,
                    }, string.Empty));

            Assert.NotNull(exception);
            Assert.Equal(QuoteInvalidTypeError, exception.Message);
        }

        private EfDeletableEntityRepository<Quote> GetQuoteRepo() => new (this.dbContext);

        private UploadQuoteService GetUploadQuoteService() => new (this.GetQuoteRepo());
    }
}
