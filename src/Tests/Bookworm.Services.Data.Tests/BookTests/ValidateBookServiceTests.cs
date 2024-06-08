namespace Bookworm.Services.Data.Tests.BookTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Tests.Shared;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.LanguageErrorMessagesConstants;

    public class ValidateBookServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public ValidateBookServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task ValidateBookShouldThrowExceptionIfBookWithTitleAlreadyExists()
        {
            var service = this.GetValidateBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.ValidateAsync("Book One", 1, 1));

            Assert.Equal(BookWithTitleExistsError, exception.Message);
        }

        [Fact]
        public async Task ValidateBookShouldThrowExceptionIfCategoryIdIsInvalid()
        {
            var service = this.GetValidateBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.ValidateAsync("New Book", 1, 7));

            Assert.Equal(CategoryNotFoundError, exception.Message);
        }

        [Fact]
        public async Task ValidateBookShouldThrowExceptionIfLanguageIdIsInvalid()
        {
            var service = this.GetValidateBookService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.ValidateAsync("New Book", 7, 1));

            Assert.Equal(LanguageNotFoundError, exception.Message);
        }

        [Fact]
        public async Task ValidateBookShouldNotThrowExceptionIfBookIsValid()
        {
            var service = this.GetValidateBookService();

            var exception = await Record.ExceptionAsync(async () => await service.ValidateAsync("New Book", 1, 1));

            Assert.Null(exception);
        }

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private LanguagesService GetLanguagesService()
            => new(new EfRepository<Language>(this.dbContext), this.GetBookRepo());

        private CategoriesService GetCategoriesService() => new(new EfRepository<Category>(this.dbContext));

        private SearchBooksService GetSearchBooksService() => new(this.GetBookRepo());

        private ValidateBookService GetValidateBookService()
            => new(this.GetLanguagesService(), this.GetCategoriesService(), this.GetSearchBooksService());
    }
}
