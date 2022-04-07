namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
    using Moq;
    using Xunit;

    public class QuotesServiceTest
    {
        private readonly List<Quote> quotesList;
        private readonly QuotesService quotesService;

        public QuotesServiceTest()
        {
            this.RegisterMappings();
            this.quotesList = new List<Quote>()
            {
                new Quote()
                {
                    Content = "First quote Content",
                    UserId = "1",
                },
                new Quote()
                {
                    Content = "Second quote Content",
                    UserId = "1",
                },
            };

            Mock<IRepository<Quote>> mockQuotesRepo = new Mock<IRepository<Quote>>();
            mockQuotesRepo.Setup(x => x.AllAsNoTracking()).Returns(this.quotesList.AsQueryable());
            mockQuotesRepo.Setup(x => x.AddAsync(It.IsAny<Quote>()))
                .Callback((Quote quote) => this.quotesList.Add(quote));

            this.quotesService = new QuotesService(mockQuotesRepo.Object);
        }

        [Fact]
        public async Task QuotesNumberMustBeRight()
        {
            await this.quotesService.AddQuoteAsync("Some Quote Content", null, null, null, "1");

            var result = this.quotesService.GetAllQuotes<QuoteViewModel>().Count();

            Assert.Equal(3, result);
        }

        [Fact]
        public void GetRandomQuoteShoulWorkCorrectly()
        {
            var quoteModel = this.quotesService.GetRandomQuote<QuoteViewModel>();
            var quote = this.quotesList.FirstOrDefault(x => x.Content == quoteModel.Content);

            Assert.NotNull(quoteModel);
            Assert.NotNull(quote);
        }

        private void RegisterMappings()
        {
            AutoMapperConfig.RegisterMappings(typeof(QuoteViewModel).GetTypeInfo().Assembly);
        }
    }
}
