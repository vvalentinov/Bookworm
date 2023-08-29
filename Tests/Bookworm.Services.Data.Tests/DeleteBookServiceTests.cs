namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Models.Books;
    using Moq;
    using Xunit;

    public class DeleteBookServiceTests
    {
        private readonly IList<Book> books;
        private readonly DeleteBookService service;

        public DeleteBookServiceTests()
        {
            this.books = new List<Book>()
            {
                new Book()
                {
                    Id = "77e6fd96-e081-441b-a349-1e6f00e8a5ca",
                    Title = "First book title",
                    Description = "First book description",
                    ImageUrl = "http://example.com/air",
                    FileUrl = "https://brother.example.org/",
                },
                new Book()
                {
                    Id = "8e5fca84-9b02-4f98-9ca1-9268f2bfb62d",
                    Title = "Second book title",
                    Description = "Second book description",
                    ImageUrl = "http://baseball.example.com/",
                    FileUrl = "https://act.example.com/",
                },
            };

            Mock<IBlobService> mockBlobService = new Mock<IBlobService>();

            Mock<IDeletableEntityRepository<Book>> mockBookRepo = new Mock<IDeletableEntityRepository<Book>>();
            mockBookRepo.Setup(x => x.All()).Returns(this.books.AsQueryable());
            mockBookRepo.Setup(x => x.Delete(It.IsAny<Book>()))
                .Callback((Book book) => this.books.Remove(book));

            this.service = new DeleteBookService(mockBookRepo.Object, mockBlobService.Object);
        }

        [Fact]
        public async Task DeleteBookShouldWorkCorrectly()
        {
            await this.service.DeleteBookAsync("77e6fd96-e081-441b-a349-1e6f00e8a5ca");

            Book book = this.books.FirstOrDefault(x => x.Id == "77e6fd96-e081-441b-a349-1e6f00e8a5ca");
            Assert.Single(this.books);
            Assert.Null(book);
        }
    }
}
