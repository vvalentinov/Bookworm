namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Books;
    using Moq;
    using Xunit;

    public class RandomBookServiceTests
    {
        private readonly List<Book> booksList;
        private readonly List<Category> categoriesList;

        private readonly RandomBookService randomBookService;

        public RandomBookServiceTests()
        {
            this.RegisterMappings();

            this.categoriesList = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Arts",
                    ImageUrl = "https://bed.example.com/",
                },
                new Category()
                {
                    Id = 2,
                    Name = "History",
                    ImageUrl = "https://www.example.com/arm/bed",
                },
                new Category()
                {
                    Id = 3,
                    Name = "Horror",
                    ImageUrl = "https://example.org/",
                },
            };

            this.booksList = new List<Book>()
            {
                new Book()
                {
                    Title = "First book title",
                    Description = "First book description",
                    ImageUrl = "https://example.org/blow.php",
                    FileUrl = "https://www.example.com/airplane.aspx",
                    CategoryId = 2,
                },
                new Book()
                {
                    Title = "Second book title",
                    Description = "Second book description",
                    ImageUrl = "https://example.org/blow",
                    FileUrl = "https://www.example.com/aspx",
                    CategoryId = 2,
                },
                new Book()
                {
                    Title = "Third book title",
                    Description = "Third book description",
                    ImageUrl = "https://example.org/blow",
                    FileUrl = "https://www.example.com/aspx",
                    CategoryId = 2,
                },
            };

            Mock<IRepository<Category>> mockCategoryRepo = new Mock<IRepository<Category>>();
            mockCategoryRepo.Setup(x => x.AllAsNoTracking()).Returns(this.categoriesList.AsQueryable());
            mockCategoryRepo.Setup(x => x.AddAsync(It.IsAny<Category>()))
                .Callback((Category category) => this.categoriesList.Add(category));

            Mock<IDeletableEntityRepository<Book>> mockBookRepo = new Mock<IDeletableEntityRepository<Book>>();
            mockBookRepo.Setup(x => x.AllAsNoTracking()).Returns(this.booksList.AsQueryable());
            mockBookRepo.Setup(x => x.AddAsync(It.IsAny<Book>()))
                .Callback((Book book) => this.booksList.Add(book));

            this.randomBookService = new RandomBookService(mockCategoryRepo.Object, mockBookRepo.Object);
        }

        [Fact]
        public void GetCategoriesShouldWorkCorrectly()
        {
            var result = this.randomBookService.GetCategories().ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal("Random", result[3].Text);
            Assert.Equal("Random", result[3].Value);
        }

        [Fact]
        public void GenerateBooksShouldWorkCorrectly()
        {
            var result = this.randomBookService.GenerateBooks("History", 2);

            Assert.Equal(2, result.Count());
        }

        private void RegisterMappings()
        {
            AutoMapperConfig.RegisterMappings(typeof(BookViewModel).GetTypeInfo().Assembly);
        }
    }
}
