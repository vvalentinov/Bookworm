namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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
            this.categoriesList = new List<Category>()
            {
                 new Category() { Id = 1, Name = "Arts & Music", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Biographies & Autobiographies", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Business & Economics", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Comics & Graphic Novels", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Cooking", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Thrillers & Crimes & Mysteries", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Health & Fitness", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "History", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Hobbies & Crafts", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Horror", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Kid's Book's", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Religion", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Romance", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Sci-Fi & Fantasy", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Science & Technology", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Self-help", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Sports", ImageUrl = "https://www.example.com/#arithmetic" },
                 new Category() { Name = "Travel", ImageUrl = "https://www.example.com/#arithmetic" },
            };

            Mock<IRepository<Category>> mockRepo = new Mock<IRepository<Category>>();
            mockRepo.Setup(x => x.AllAsNoTracking()).Returns(this.categoriesList.AsQueryable());
            mockRepo.Setup(x => x.AddAsync(It.IsAny<Category>()))
                .Callback((Category category) => this.categoriesList.Add(category));

            this.categoriesService = new CategoriesService(mockRepo.Object);
        }

        [Fact]
        public void GetAllCategoriesShouldWorkCorrectly()
        {
            var categories = this.categoriesService.GetAll<CategoryViewModel>().ToList();
            Assert.Equal(18, categories.Count());
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
