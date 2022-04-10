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
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Quotes;
    using Moq;
    using Xunit;

    public class QuotesServiceTest
    {
        private readonly List<Quote> quotesList;
        private readonly List<ApplicationUser> usersList;
        private readonly QuotesService quotesService;

        public QuotesServiceTest()
        {
            this.RegisterMappings();
            this.usersList = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "567b180c-85d6-4f22-953e-4790431e957f",
                    Points = 5,
                },
                new ApplicationUser()
                {
                    Id = "7ef3646c-3aec-43dc-8f97-74afbe3aed11",
                },
                new ApplicationUser()
                {
                    Id = "52870096-5c2e-4b5a-9b10-d3285fe79a22",
                },
            };
            this.quotesList = new List<Quote>()
            {
                new Quote()
                {
                    Id = 4,
                    Content = "First quote Content",
                    UserId = "567b180c-85d6-4f22-953e-4790431e957f",
                    BookTitle = "First quote book title",
                    AuthorName = "First quote author name",
                    MovieTitle = "First quote movie title",
                    IsApproved = true,
                },
                new Quote()
                {
                    Id = 10,
                    Content = "Second quote Content",
                    UserId = "567b180c-85d6-4f22-953e-4790431e957f",
                    BookTitle = "Second quote book title",
                    AuthorName = "Second quote author name",
                    MovieTitle = "Second quote movie title",
                    IsApproved = false,
                },
                new Quote()
                {
                    Id = 1,
                    Content = "Third quote Content",
                    UserId = "7ef3646c-3aec-43dc-8f97-74afbe3aed11",
                    BookTitle = "Third quote book title",
                    AuthorName = "Third quote author name",
                    MovieTitle = "Third quote movie title",
                    IsApproved = true,
                },
                new Quote()
                {
                    Id = 6,
                    Content = "Fourth quote Content",
                    UserId = "52870096-5c2e-4b5a-9b10-d3285fe79a22",
                    BookTitle = "Fourth quote book title",
                    AuthorName = "Fourth quote author name",
                    MovieTitle = "Fourth quote movie title",
                    IsApproved = false,
                },
            };

            Mock<IEmailSender> mockEmailSender = new Mock<IEmailSender>();

            Mock<IDeletableEntityRepository<ApplicationUser>> mockUserRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            mockUserRepo.Setup(x => x.All()).Returns(this.usersList.AsQueryable());

            Mock<IRepository<Quote>> mockQuotesRepo = new Mock<IRepository<Quote>>();
            mockQuotesRepo.Setup(x => x.AllAsNoTracking()).Returns(this.quotesList.AsQueryable());
            mockQuotesRepo.Setup(x => x.All()).Returns(this.quotesList.AsQueryable());
            mockQuotesRepo.Setup(x => x.AddAsync(It.IsAny<Quote>()))
                .Callback((Quote quote) => this.quotesList.Add(quote));
            mockQuotesRepo.Setup(x => x.Delete(It.IsAny<Quote>()))
                .Callback((Quote quote) => this.quotesList.Remove(quote));

            this.quotesService = new QuotesService(mockQuotesRepo.Object, mockEmailSender.Object, mockUserRepo.Object);
        }

        [Fact]
        public async Task QuotesNumberMustBeRight()
        {
            await this.quotesService.AddQuoteAsync(
                "Some Quote Content",
                "Some author name",
                "Some book title",
                "Some movie title",
                "567b180c-85d6-4f22-953e-4790431e957f");

            int result = this.quotesService.GetAllQuotes<QuoteViewModel>().Count();

            Assert.Equal(2, result);
        }

        [Fact]
        public void GetRandomQuoteShoulWorkCorrectly()
        {
            QuoteViewModel quoteModel = this.quotesService.GetRandomQuote<QuoteViewModel>();
            Quote quote = this.quotesList.FirstOrDefault(x => x.Content == quoteModel.Content);

            Assert.NotNull(quoteModel);
            Assert.NotNull(quote);
        }

        [Fact]
        public void GetAllUnapprovedQuotesShouldWorkCorrectly()
        {
            IEnumerable<QuoteViewModel> quotes = this.quotesService.GetAllUnapprovedQuotes<QuoteViewModel>();

            Assert.Equal(2, quotes.Count());
        }

        [Fact]
        public void GetQuoteByIdShouldWorkCorrectly()
        {
            QuoteViewModel quote = this.quotesService.GetQuoteById(10);

            Assert.NotNull(quote);
            Assert.Equal("Second quote Content", quote.Content);
            Assert.Equal("Second quote book title", quote.BookTitle);
            Assert.Equal("Second quote author name", quote.AuthorName);
            Assert.Equal("Second quote movie title", quote.MovieTitle);
            Assert.Equal(10, quote.Id);
        }

        [Fact]
        public void GetUserQuotesShouldWorkCorrectly()
        {
            List<QuoteViewModel> quotes = this.quotesService.GetUserQuotes("567b180c-85d6-4f22-953e-4790431e957f").ToList();

            Assert.Equal(2, quotes.Count());

            Assert.Equal("First quote Content", quotes[0].Content);
            Assert.Equal("First quote book title", quotes[0].BookTitle);
            Assert.Equal("First quote author name", quotes[0].AuthorName);
            Assert.Equal("First quote movie title", quotes[0].MovieTitle);
            Assert.Equal(4, quotes[0].Id);
            Assert.IsType<QuoteViewModel>(quotes[0]);

            Assert.Equal("Second quote Content", quotes[1].Content);
            Assert.Equal("Second quote book title", quotes[1].BookTitle);
            Assert.Equal("Second quote author name", quotes[1].AuthorName);
            Assert.Equal("Second quote movie title", quotes[1].MovieTitle);
            Assert.Equal(10, quotes[1].Id);
            Assert.IsType<QuoteViewModel>(quotes[1]);
        }

        [Fact]
        public async Task EditQuoteShouldWordCorrectly()
        {
            await this.quotesService.EditQuoteAsync(
                4,
                "New quote content",
                "New quote Author",
                "New quote Book Title",
                "New quote Movie Title");

            Quote quote = this.quotesList[0];

            Assert.Equal("New quote content", quote.Content);
            Assert.Equal("New quote Author", quote.AuthorName);
            Assert.Equal("New quote Book Title", quote.BookTitle);
            Assert.Equal("New quote Movie Title", quote.MovieTitle);
        }

        [Fact]
        public async Task DeleteQuoteShouldWordCorrectly()
        {
            await this.quotesService.DeleteQuoteAsync(4);

            Quote quote = this.quotesList.FirstOrDefault(x => x.Id == 4);
            Assert.Equal(3, this.quotesList.Count);
            Assert.Null(quote);
        }

        [Fact]
        public async Task ApproveQuoteShouldWorkCorrectly()
        {
            await this.quotesService.ApproveQuote(10, "567b180c-85d6-4f22-953e-4790431e957f");

            var quote = this.quotesList.First(x => x.Id == 10);
            var user = this.usersList.First(x => x.Id == "567b180c-85d6-4f22-953e-4790431e957f");

            Assert.True(quote.IsApproved);
            Assert.Equal(8, user.Points);
        }

        private void RegisterMappings()
        {
            AutoMapperConfig.RegisterMappings(typeof(QuoteViewModel).GetTypeInfo().Assembly);
        }
    }
}
