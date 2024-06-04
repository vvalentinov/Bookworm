namespace Bookworm.Services.Data.Tests
{
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Xunit;

    public class PublishersServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public PublishersServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetPublisherWithNameShouldWorkCorrectly()
        {
            var publisherService = this.GetPublishersService();

            var publisher = await publisherService.GetPublisherWithNameAsync("Publisher One");

            Assert.NotNull(publisher);
        }

        [Fact]
        public async Task GetPublisherWithNameShouldReturnNullIfNoPublisherIsFound()
        {
            var publisherService = this.GetPublishersService();

            var publisher = await publisherService.GetPublisherWithNameAsync("Publisher Ten");

            Assert.Null(publisher);
        }

        private PublishersService GetPublishersService() => new (new EfRepository<Publisher>(this.dbContext));
    }
}
