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
    using Bookworm.Web.ViewModels.Categories;
    using Moq;
    using Xunit;

    public class CategoriesServiceTests
    {
        private readonly List<Category> categoriesList;
        private readonly CategoriesService categoriesService;

        public CategoriesServiceTests()
        {
            this.RegisterMappings();
            this.categoriesList =
            [
                 new () { Id = 1, Name = "Arts & Music" },
                 new () { Id = 2, Name = "Biographies & Autobiographies" },
                 new () { Id = 3, Name = "Business & Economics" },
                 new () { Id = 4, Name = "Comics & Graphic Novels" },
                 new () { Id = 5, Name = "Cooking" },
                 new () { Id = 6, Name = "Thrillers & Crimes & Mysteries" },
                 new () { Id = 7, Name = "Health & Fitness" },
                 new () { Id = 8, Name = "History" },
                 new () { Id = 9, Name = "Hobbies & Crafts" },
                 new () { Id = 10, Name = "Horror" },
                 new () { Id = 11, Name = "Kid's Book's" },
                 new () { Id = 12, Name = "Religion" },
                 new () { Id = 13, Name = "Romance" },
                 new () { Id = 14, Name = "Sci-Fi & Fantasy" },
                 new () { Id = 15, Name = "Science & Technology" },
                 new () { Id = 16, Name = "Self-help" },
                 new () { Id = 17, Name = "Sports" },
                 new () { Id = 18, Name = "Travel" },
            ];

            Mock<IRepository<Category>> mockCategoriesRepo = new Mock<IRepository<Category>>();
            mockCategoriesRepo.Setup(x => x.AllAsNoTracking()).Returns(this.categoriesList.AsQueryable());
            mockCategoriesRepo.Setup(x => x.AddAsync(It.IsAny<Category>()))
                .Callback((Category category) => this.categoriesList.Add(category));

            this.categoriesService = new CategoriesService(mockCategoriesRepo.Object);
        }

        [Fact]
        public async Task GetAllCategoriesShouldWorkCorrectly()
        {
            List<CategoryViewModel> categories = await this.categoriesService.GetAllAsync<CategoryViewModel>();
            Assert.Equal(18, categories.Count);
        }

        // [Fact]
        // public void GetCategoriesAsSelectListItemsShouldWorkCorrectly()
        // {
        //    IEnumerable<SelectListItem> categories = this.categoriesService.GetCategoriesAsSelectListItems();

        // Assert.Equal(18, categories.Count());
        //    foreach (var category in categories)
        //    {
        //        Assert.IsType<SelectListItem>(category);
        //    }
        // }
        [Fact]
        public void GetCategoryNameShouldWorkCorrectly()
        {
            // string categoryName = this.categoriesService.GetCategoryName(5);
            // Assert.Equal("Cooking", categoryName);
        }

        [Fact]
        public async void GetCategoryIdShouldWorkCorrectly()
        {
            int categoryId = await this.categoriesService.GetCategoryIdAsync("Arts & Music");
            Assert.Equal(1, categoryId);
        }

        private void RegisterMappings()
        {
            AutoMapperConfig.RegisterMappings(typeof(CategoryViewModel).GetTypeInfo().Assembly);
        }
    }
}
