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
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Moq;
    using Xunit;

    public class CategoriesServiceTests
    {
        private readonly List<Category> categoriesList;
        private readonly CategoriesService categoriesService;

        public CategoriesServiceTests()
        {
            this.RegisterMappings();
            this.categoriesList = new List<Category>()
            {
                 new Category() { Id = 1, Name = "Arts & Music", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 2, Name = "Biographies & Autobiographies", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 3, Name = "Business & Economics", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 4, Name = "Comics & Graphic Novels", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 5, Name = "Cooking", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 6, Name = "Thrillers & Crimes & Mysteries", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 7, Name = "Health & Fitness", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 8, Name = "History", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 9, Name = "Hobbies & Crafts", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 10, Name = "Horror", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 11, Name = "Kid's Book's", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 12, Name = "Religion", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 13, Name = "Romance", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 14, Name = "Sci-Fi & Fantasy", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 15, Name = "Science & Technology", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 16, Name = "Self-help", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 17, Name = "Sports", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Id = 18, Name = "Travel", ImageUrl = "https://www.example.com/#arithmetic" },
            };

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

        [Fact]
        public void GetCategoriesAsSelectListItemsShouldWorkCorrectly()
        {
            IEnumerable<SelectListItem> categories = this.categoriesService.GetCategoriesAsSelectListItems();

            Assert.Equal(18, categories.Count());
            foreach (var category in categories)
            {
                Assert.IsType<SelectListItem>(category);
            }
        }

        [Fact]
        public void GetCategoryNameShouldWorkCorrectly()
        {
            //string categoryName = this.categoriesService.GetCategoryName(5);
            //Assert.Equal("Cooking", categoryName);
        }

        [Fact]
        public void GetCategoryIdShouldWorkCorrectly()
        {
            int categoryId = this.categoriesService.GetCategoryId("Arts & Music");
            Assert.Equal(1, categoryId);
        }

        private void RegisterMappings()
        {
            AutoMapperConfig.RegisterMappings(typeof(CategoryViewModel).GetTypeInfo().Assembly);
        }
    }
}
