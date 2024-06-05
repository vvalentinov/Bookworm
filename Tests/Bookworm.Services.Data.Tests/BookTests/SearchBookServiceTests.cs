namespace Bookworm.Services.Data.Tests.BookTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Web.ViewModels.Books;
    using Xunit;

    public class SearchBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public SearchBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task CheckIfBookWithTitleExistsShouldWorkCorrectly(int? bookId)
        {
            var service = this.GetSearchBooksService();
            var title = "Book One";

            var bookExists = await service.CheckIfBookWithTitleExistsAsync(title, bookId);

            if (bookId.HasValue)
            {
                Assert.False(bookExists);
            }
            else
            {
                Assert.True(bookExists);
            }
        }

        [Fact]
        public async Task SearchBooksShouldWorkCorrectly()
        {
            var service = this.GetSearchBooksService();

            var model = new SearchBookInputModel
            {
                Page = 1,
                CategoryId = 3,
                Input = "boOk ONE",
                IsForUserBooks = false,
                LanguagesIds = new List<int> { 1, 2 },
            };

            var result = await service.SearchBooksAsync(model);

            Assert.Single(result.Books);
            Assert.Equal("Book One", result.Books.First().Title);
        }

        [Fact]
        public async Task SearchUserBooksShouldWorkCorrectly()
        {
            var service = this.GetSearchBooksService();
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var model = new SearchBookInputModel
            {
                Page = 1,
                CategoryId = 5,
                UserId = userId,
                Input = "bOOk TEN",
                IsForUserBooks = true,
                LanguagesIds = new List<int>(),
            };

            var result = await service.SearchBooksAsync(model);

            Assert.Single(result.Books);
            Assert.Equal("Book Ten", result.Books.First().Title);
        }

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private SearchBooksService GetSearchBooksService() => new(this.GetBookRepo());
    }
}
