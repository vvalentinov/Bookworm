namespace Bookworm.Services.Data.Tests
{
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Xunit;

    public class AuthorsServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public AuthorsServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetAuthorWithNameShouldWorkCorrectly()
        {
            var authorsService = this.GetAuthorsService();

            var author = await authorsService.GetAuthorWithNameAsync("Author One");

            Assert.NotNull(author);
        }

        [Fact]
        public async Task GetAuthorWithNameShouldReturnNullIfNoAuthorIsFound()
        {
            var authorsService = this.GetAuthorsService();

            var author = await authorsService.GetAuthorWithNameAsync("Author Ten");

            Assert.Null(author);
        }

        private AuthorsService GetAuthorsService() => new (new EfRepository<Author>(this.dbContext));
    }
}
