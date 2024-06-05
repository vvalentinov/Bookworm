namespace Bookworm.Services.Data.Tests.BookTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class UploadBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public UploadBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Theory]
        [InlineData("Some Title One", "Publisher One")]
        [InlineData("Some Title Two", "Publisher Six")]
        public async Task UploadBookShouldWorkCorrectly(string bookTitle, string publisherName)
        {
            var booKRepo = this.GetBookRepo();
            var service = this.GetUploadBookService();
            var publisherRepo = this.GetPublisherRepo();

            var bookDto = new BookDto
            {
                Year = 2012,
                CategoryId = 2,
                LanguageId = 3,
                PagesCount = 12,
                Title = bookTitle,
                Publisher = publisherName,
                BookFile = this.GetFile(),
                ImageFile = this.GetFile(),
                Description = "Some Description",
                Authors = new List<UploadAuthorViewModel>
                {
                    new() { Name = "Author One" },
                    new() { Name = "Author Two" },
                },
            };

            await service.UploadBookAsync(bookDto, "0fc3ea28-3165-440e-947e-670c90562320");

            var book = await booKRepo
                .AllAsNoTracking()
                .Include(x => x.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Title == bookTitle);

            var publisher = await publisherRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == publisherName);

            var authorsIds = book.AuthorsBooks.Select(x => x.AuthorId).OrderBy(x => x).ToList();

            Assert.NotNull(book);
            Assert.NotNull(publisher);
            Assert.Equal(1, authorsIds[0]);
            Assert.Equal(2, authorsIds[1]);
        }

        private IFormFile GetFile() => new Mock<IFormFile>().Object;

        private EfRepository<Publisher> GetPublisherRepo() => new(this.dbContext);

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private AuthorsService GetAuthorsService() => new(new EfRepository<Author>(this.dbContext));

        private PublishersService GetPublishersService() => new(new EfRepository<Publisher>(this.dbContext));

        private IValidateBookService GetValidateBookService()
        {
            var validateServiceMock = new Mock<IValidateBookService>();

            validateServiceMock
                .Setup(x => x.ValidateAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int?>()));

            return validateServiceMock.Object;
        }

        private IBlobService GetBlobService()
        {
            var blobServiceMock = new Mock<IBlobService>();

            blobServiceMock
                .Setup(x => x.UploadBlobAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("https://some-url-here");

            return blobServiceMock.Object;
        }

        private UploadBookService GetUploadBookService()
            => new(this.GetBlobService(),
                this.GetAuthorsService(),
                this.GetPublishersService(),
                this.GetValidateBookService(),
                this.GetBookRepo());
    }
}
