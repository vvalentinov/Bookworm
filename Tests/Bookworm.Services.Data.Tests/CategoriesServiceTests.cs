namespace Bookworm.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Web.ViewModels.Categories;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;

    public class CategoriesServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public CategoriesServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task GetAllShouldWorkCorrectly()
        {
            var service = this.GetCategoriesService();

            var categories = await service.GetAllAsync<CategoryViewModel>();

            Assert.Equal(5, categories.Count());
        }

        [Fact]
        public async Task GetCategoryIdShouldWorkCorrectly()
        {
            var service = this.GetCategoriesService();

            var id = await service.GetCategoryIdAsync("  Category One ");

            Assert.Equal(1, id);
        }

        [Fact]
        public async Task GetCategoryIdShouldThrowExceptionIfCategoryNameIsInvalid()
        {
            var service = this.GetCategoriesService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetCategoryIdAsync("Invalid Category Name"));

            Assert.Equal(CategoryNotFoundError, exception.Message);
        }

        [Fact]
        public async Task CheckIfIdIsValidShouldReturnTrueIfIdIsValid()
        {
            var service = this.GetCategoriesService();

            var isValid = await service.CheckIfIdIsValidAsync(1);

            Assert.True(isValid);
        }

        [Fact]
        public async Task CheckIfIdIsValidShouldReturnFalseIfIdIsNotValid()
        {
            var service = this.GetCategoriesService();

            var isValid = await service.CheckIfIdIsValidAsync(6);

            Assert.False(isValid);
        }

        private CategoriesService GetCategoriesService() => new(new EfRepository<Category>(this.dbContext));
    }
}
