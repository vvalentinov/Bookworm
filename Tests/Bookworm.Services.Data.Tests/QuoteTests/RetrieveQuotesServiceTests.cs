namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels;
    using Bookworm.Web.ViewModels.DTOs;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class RetrieveQuotesServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public RetrieveQuotesServiceTests(DbContextFixture dbContextFixture)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetAllApprovedQuotesShouldWorkCorrectly()
        {
            var model = await this.GetRetrieveQuotesService()
                .GetAllApprovedAsync();

            Assert.Equal(9, model.Quotes.Count);
            Assert.Equal(2, model.PagesCount);
        }

        [Fact]
        public async Task GetAllApprovedQuotesShouldWorkCorrectlyWithPageParameter()
        {
            var model = await this.GetRetrieveQuotesService()
                .GetAllApprovedAsync(2);

            Assert.Equal(3, model.Quotes.Count);
            Assert.Equal(2, model.PagesCount);
        }

        [Fact]
        public async Task GetAllApprovedQuotesShouldWorkCorrectlyWithUserIdParameter()
        {
            var retrieveQuotesService = this.GetRetrieveQuotesService();
            string userId = "0fc3ea28-3165-440e-947e-670c90562320";

            var model = await retrieveQuotesService.GetAllApprovedAsync(null, userId);

            var quotes = model.Quotes.Where(q => q.Id == 7 || q.Id == 10);
            var userQuotes = model.Quotes.Where(q => q.UserId == userId);

            foreach (var quote in quotes)
            {
                Assert.True(quote.IsLikedByUser);
                Assert.False(quote.IsUserQuoteCreator);
            }

            foreach (var quote in userQuotes)
            {
                Assert.False(quote.IsLikedByUser);
                Assert.True(quote.IsUserQuoteCreator);
            }
        }

        [Fact]
        public async Task GetAllUnapprovedQuotesShouldWorkCorrectly()
        {
            var model = await this.GetRetrieveQuotesService()
                .GetAllUnapprovedAsync();

            var quoteIds = new[] { 1, 4, 9 };

            foreach (var quote in model.Quotes)
            {
                Assert.Contains(quote.Id, quoteIds);
            }
        }

        [Fact]
        public async Task GetUnapprovedCountShouldWorkCorrectly()
        {
            int count = await this.GetRetrieveQuotesService()
                .GetUnapprovedCountAsync();

            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetAllDeletedShouldWorkCorrectly()
        {
            var model = await this.GetRetrieveQuotesService()
                .GetAllDeletedAsync();

            Assert.Equal(2, model.Quotes.Count);

            var quoteIds = new[] { 6, 13 };

            foreach (var quote in model.Quotes)
            {
                Assert.Contains(quote.Id, quoteIds);
            }
        }

        [Fact]
        public async Task GetByIdShouldWorkCorrectly()
        {
            var quote = await this.GetRetrieveQuotesService()
                .GetByIdAsync(1);

            Assert.NotNull(quote);
            Assert.Equal(1, quote.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(15)]
        [InlineData(-12)]
        public async Task GetByIdShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await this.GetRetrieveQuotesService().GetByIdAsync(id));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetRandomQuoteShouldWorkCorrectly()
        {
            var quote = await this.GetRetrieveQuotesService()
                .GetRandomAsync();

            Assert.NotNull(quote);
            Assert.True(quote.IsApproved);
        }

        [Fact]
        public async Task GetAllUserQuotesShouldWorkCorrectly()
        {
            var model = await this.GetRetrieveQuotesService()
                .GetAllUserQuotesAsync("0fc3ea28-3165-440e-947e-670c90562320", 1);

            Assert.Equal(5, model.Quotes.Count);
        }

        [Fact]
        public async Task GetQuoteForEditShouldWorkCorrectly()
        {
            var quote = await this.GetRetrieveQuotesService()
                .GetQuoteForEditAsync(1, "0fc3ea28-3165-440e-947e-670c90562320");

            Assert.NotNull(quote);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-12)]
        [InlineData(20)]
        public async Task GetQuoteForEditShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await this.GetRetrieveQuotesService().GetQuoteForEditAsync(id, string.Empty));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetQuoteForEditShouldThrowExceptionIfCurrUserIsNotCreator()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
               await this.GetRetrieveQuotesService().GetQuoteForEditAsync(3, "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.Equal(QuoteEditError, exception.Message);
        }

        [Theory]
        [InlineData("MovieQuote")]
        [InlineData("BookQuote")]
        [InlineData("GeneralQuote")]
        [InlineData("LikedQuote")]
        public async Task GetAllByTypeShouldWorkCorrectly(string type)
        {
            var getQuotesDto = new GetQuotesApiDto { SortCriteria = "NewestToOldest", Type = type };

            var model = await this.GetRetrieveQuotesService()
                .GetAllByCriteriaAsync("0fc3ea28-3165-440e-947e-670c90562320", getQuotesDto);

            switch (type)
            {
                case "MovieQuote":
                    Assert.Equal(4, model.Quotes.Count);
                    Assert.Equal("Here's looking at you, kid.", model.Quotes[0].Content);
                    break;
                case "BookQuote":
                    Assert.Equal(3, model.Quotes.Count);
                    Assert.Equal("It is nothing to die; it is dreadful not to live.", model.Quotes[0].Content);
                    break;
                case "GeneralQuote":
                    Assert.Equal(2, model.Quotes.Count);
                    Assert.Equal("The future belongs to those who believe in the beauty of their dreams", model.Quotes[0].Content);
                    break;
                case "LikedQuote":
                    Assert.Equal(2, model.Quotes.Count);
                    Assert.Equal("Here's looking at you, kid.", model.Quotes[0].Content);
                    Assert.Equal("Elementary, my dear Watson", model.Quotes[1].Content);
                    break;
            }
        }

        [Theory]
        [InlineData("NewestToOldest")]
        [InlineData("OldestToNewest")]
        [InlineData("LikesCountDesc")]
        public async Task GetAllBySortCriteriaShouldWorkCorrectly(string sortCriteria)
        {
            var getQuotesDto = new GetQuotesApiDto { SortCriteria = sortCriteria };

            var model = await this.GetRetrieveQuotesService()
                .GetAllByCriteriaAsync("0fc3ea28-3165-440e-947e-670c90562320", getQuotesDto);

            switch (sortCriteria)
            {
                case "NewestToOldest":
                    Assert.Equal("It is nothing to die; it is dreadful not to live.", model.Quotes[0].Content);
                    Assert.Equal("I took a deep breath and listened to the old brag of my heart: I am, I am, I am.", model.Quotes[1].Content);
                    break;
                case "OldestToNewest":
                    Assert.Equal("The way to get started is to quit talking and begin doing", model.Quotes[0].Content);
                    Assert.Equal("The future belongs to those who believe in the beauty of their dreams", model.Quotes[1].Content);
                    break;
                case "LikesCountDesc":
                    Assert.Equal("Elementary, my dear Watson", model.Quotes[0].Content);
                    Assert.Equal("Here's looking at you, kid.", model.Quotes[1].Content);
                    break;
            }
        }

        [Theory]
        [InlineData("hErE")]
        [InlineData("  ELeMeNTArY, my DEaR WatSON  ")]
        public async Task GetAllQuotesBySearchContentWithoutTypeShouldWorkCorrectly(string content)
        {
            var getQuotesDto = new GetQuotesApiDto { SortCriteria = "NewestToOldest", Content = content };

            var model = await this.GetRetrieveQuotesService()
                .GetAllByCriteriaAsync("0fc3ea28-3165-440e-947e-670c90562320", getQuotesDto);

            switch (content)
            {
                case "hErE":
                    Assert.Equal(2, model.Quotes.Count);
                    Assert.Equal("Here's looking at you, kid.", model.Quotes[0].Content);
                    Assert.Equal("There's no place like home", model.Quotes[1].Content);
                    break;
                case "  ELeMeNTArY, my DEaR WatSON  ":
                    Assert.Single(model.Quotes);
                    Assert.Equal("Elementary, my dear Watson", model.Quotes[0].Content);
                    break;
            }
        }

        [Fact]
        public async Task GetAllQuotesBySearchMovieTitleWithoutTypeShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto { SortCriteria = "NewestToOldest", Content = " THE WiZaRd OF Oz" };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(string.Empty, getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("There's no place like home", model.Quotes[0].Content);
        }

        [Fact]
        public async Task GetAllQuotesBySearchBookTitleWithoutTypeShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Content = " THE PeRkS of BeiNg A WallfloWER ",
            };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(string.Empty, getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("We accept the love we think we deserve.", model.Quotes[0].Content);
        }

        [Fact]
        public async Task GetAllQuotesBySearchAuthorNameWithoutTypeShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Content = " SylViA PlATh   ",
            };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(string.Empty, getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("I took a deep breath and listened to the old brag of my heart: I am, I am, I am.", model.Quotes[0].Content);
        }

        [Fact]
        public async Task GetAllQuotesBySearchMovieTitleInMovieQuoteTypeCriteriaShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Content = "  FrAnKeNStEiN  ",
                Type = "MovieQuote",
            };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(string.Empty, getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("It's alive! It's alive!", model.Quotes[0].Content);
        }

        [Fact]
        public async Task GetAllQuotesBySearchAuthorInGeneralQuoteTypeShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Content = " WaLT DisNEY   ",
                Type = "GeneralQuote",
            };

            var model = await this.GetRetrieveQuotesService()
                .GetAllByCriteriaAsync("0fc3ea28-3165-440e-947e-670c90562320", getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("The way to get started is to quit talking and begin doing", model.Quotes[0].Content);
        }

        [Fact]
        public async Task GetAllQuotesBySearchAuthorInBookQuoteTypeShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Content = " StEPhEn ChBoSkY   ",
                Type = "BookQuote",
            };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(string.Empty, getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("We accept the love we think we deserve.", model.Quotes[0].Content);
        }

        [Fact]
        public async Task GetAllQuotesBySearchBookTitleInBookQuoteTypeShouldWorkCorrectly()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Content = "  THe BeLl JAr   ",
                Type = "BookQuote",
            };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(string.Empty, getQuotesDto);

            Assert.Single(model.Quotes);
            Assert.Equal("I took a deep breath and listened to the old brag of my heart: I am, I am, I am.", model.Quotes[0].Content);
        }

        [Theory]
        [InlineData("Approved", "0fc3ea28-3165-440e-947e-670c90562320")]
        [InlineData("Unapproved", "0fc3ea28-3165-440e-947e-670c90562320")]
        public async Task GetAllUserQuotesByStatusShouldWorkCorrectly(string status, string userId)
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                QuoteStatus = status,
                IsForUserQuotes = true,
            };

            var model = await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(userId, getQuotesDto);

            if (status == "Approved")
            {
                Assert.Equal(4, model.Quotes.Count);
            }
            else
            {
                Assert.Single(model.Quotes);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Invalid Sort")]
        public async Task GetAllByCriteriaShouldThrowExceptionIfSortIsInvalid(string sortCriteria)
        {
            var getQuotesDto = new GetQuotesApiDto { SortCriteria = sortCriteria };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(
                    string.Empty, getQuotesDto));

            Assert.Equal("Invalid sort quote criteria!", exception.Message);
        }

        [Fact]
        public async Task GetAllByCriteriaShouldThrowExceptionIfStatusIsInvalid()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                QuoteStatus = "Invalid Status",
                IsForUserQuotes = true,
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(
                    "0fc3ea28-3165-440e-947e-670c90562320", getQuotesDto));

            Assert.Equal("Invalid quote status!", exception.Message);
        }

        [Fact]
        public async Task GetAllByCriteriaShouldThrowExceptionIfTypeIsInvalid()
        {
            var getQuotesDto = new GetQuotesApiDto
            {
                SortCriteria = "NewestToOldest",
                Type = "Invalid Type",
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.GetRetrieveQuotesService().GetAllByCriteriaAsync(
                    "0fc3ea28-3165-440e-947e-670c90562320", getQuotesDto));

            Assert.Equal("Invalid quote type!", exception.Message);
        }

        private EfRepository<QuoteLike> GetQuoteLikeRepo()
            => new EfRepository<QuoteLike>(this.dbContext);

        private EfDeletableEntityRepository<Quote> GetQuoteRepo()
            => new EfDeletableEntityRepository<Quote>(this.dbContext);

        private RetrieveQuotesService GetRetrieveQuotesService()
            => new RetrieveQuotesService(this.GetQuoteRepo(), this.GetQuoteLikeRepo());
    }
}
