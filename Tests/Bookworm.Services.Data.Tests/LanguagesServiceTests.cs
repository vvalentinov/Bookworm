namespace Bookworm.Services.Data.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.CodeAnalysis;
    using Xunit;

    public class LanguagesServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public LanguagesServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetAllShouldWorkCorrectly()
        {
            var service = this.GetLanguagesService();

            var languages = await service.GetAllAsync();

            Assert.Equal(5, languages.Count());
        }

        [Fact]
        public async Task GetAllInBookCategoryShouldWorkCorrectly()
        {
            var service = this.GetLanguagesService();

            var languages = await service.GetAllInBookCategoryAsync(5);
            var languagesNames = languages.Select(x => x.Name);

            Assert.Equal(3, languages.Count());
            Assert.Contains("Language Two", languagesNames);
            Assert.Contains("Language Three", languagesNames);
            Assert.Contains("Language Five", languagesNames);
        }

        [Fact]
        public async Task GetAllInUserBooksShouldWorkCorrectly()
        {
            var service = this.GetLanguagesService();

            var languages = await service.GetAllInUserBooksAsync("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");
            var languagesNames = languages.Select(x => x.Name);

            Assert.Equal(2, languages.Count());
            Assert.Contains("Language Three", languagesNames);
            Assert.Contains("Language Five", languagesNames);
        }

        [Fact]
        public async Task CheckIfIdIsValidShouldReturnTrueIfIdIsValid()
        {
            var service = this.GetLanguagesService();

            var isValid = await service.CheckIfIdIsValidAsync(1);

            Assert.True(isValid);
        }

        [Fact]
        public async Task CheckIfIdIsValidShouldReturnFalseIfIdIsNotValid()
        {
            var service = this.GetLanguagesService();

            var isValid = await service.CheckIfIdIsValidAsync(10);

            Assert.False(isValid);
        }

        private LanguagesService GetLanguagesService()
            => new (new EfRepository<Language>(this.dbContext), new EfDeletableEntityRepository<Book>(this.dbContext));
    }
}
