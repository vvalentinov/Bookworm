namespace Bookworm.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Web.ViewModels.Books;
    using Moq;
    using Xunit;

    public class FavoriteBooksServiceTests
    {
        private readonly List<FavoriteBook> favoriteBooksList;
        private readonly List<Book> booksList;
        private readonly FavoriteBookService favoriteBooksService;

        public FavoriteBooksServiceTests()
        {
            this.favoriteBooksList = new List<FavoriteBook>()
            {
                new FavoriteBook()
            {
                BookId = "18d62334-6257-4317-aa91-d5cd05ee5fd0",
                UserId = "978bc407-6236-46ac-80e1-3b3614a8dd05",
            },
                new FavoriteBook()
                {
                    BookId = "b3941efd-c7be-4e56-9627-ac58dbbf4c0e",
                    UserId = "978bc407-6236-46ac-80e1-3b3614a8dd05",
                },
            };

            this.booksList = new List<Book>();

            Mock<IRepository<FavoriteBook>> mockFaverotiteBookRepo = new Mock<IRepository<FavoriteBook>>();
            mockFaverotiteBookRepo.Setup(x => x.AllAsNoTracking()).Returns(this.favoriteBooksList.AsQueryable());
            mockFaverotiteBookRepo.Setup(x => x.All()).Returns(this.favoriteBooksList.AsQueryable());
            mockFaverotiteBookRepo.Setup(x => x.AddAsync(It.IsAny<FavoriteBook>()))
                .Callback((FavoriteBook favoriteBook) => this.favoriteBooksList.Add(favoriteBook));
            mockFaverotiteBookRepo.Setup(x => x.Delete(It.IsAny<FavoriteBook>()))
                .Callback((FavoriteBook favoriteBook) => this.favoriteBooksList.Remove(favoriteBook));

            Mock<IDeletableEntityRepository<Book>> mockBookRepository = new Mock<IDeletableEntityRepository<Book>>();
            mockBookRepository.Setup(x => x.AllAsNoTracking()).Returns(this.booksList.AsQueryable());
            mockBookRepository.Setup(x => x.AddAsync(It.IsAny<Book>()))
                .Callback((Book book) => this.booksList.Add(book));

            this.favoriteBooksService = new FavoriteBookService(mockFaverotiteBookRepo.Object, mockBookRepository.Object);
        }

        [Fact]
        public async Task AddBookToFaboritesShouldThrowExceptionIfBookIsAlreadyPresentInCollection()
        {
            var exception = await Assert.ThrowsAsync<Exception>(() => this.favoriteBooksService.AddBookToFavoritesAsync("18d62334-6257-4317-aa91-d5cd05ee5fd0", "978bc407-6236-46ac-80e1-3b3614a8dd05"));
            Assert.Equal("This book is already present in favorites!", exception.Message);
        }

        [Fact]
        public async Task AddBookToFavoritesShouldWorkCorrectly()
        {
            await this.favoriteBooksService.AddBookToFavoritesAsync("9b7d874a-ff7d-49e6-aba9-e22b2f380f28", "978bc407-6236-46ac-80e1-3b3614a8dd05");
            Assert.Equal(3, this.favoriteBooksList.Count());
        }

        [Fact]
        public async Task DeletingBookFromFavoritesCollectionShouldWorkCorrectly()
        {
            await this.favoriteBooksService.DeleteFromFavoritesAsync("18d62334-6257-4317-aa91-d5cd05ee5fd0", "978bc407-6236-46ac-80e1-3b3614a8dd05");
            Assert.Single(this.favoriteBooksList);
        }

        [Fact]
        public void GetUserFavoriteBooksShouldWorkCorrectly()
        {
            Book bookOne = new Book()
            {
                Id = "18d62334-6257-4317-aa91-d5cd05ee5fd0",
                Description = "First book description",
                FileUrl = "https://www.example.com/bridge.aspx",
                ImageUrl = "https://example.net/alarm/beef#argument",
                Title = "First book title",
            };

            Book bookTwo = new Book()
            {
                Id = "b3941efd-c7be-4e56-9627-ac58dbbf4c0e",
                Description = "Second book description",
                FileUrl = "http://board.example.com/bat",
                ImageUrl = "http://example.com/",
                Title = "Second book title",
            };

            this.booksList.Add(bookOne);
            this.booksList.Add(bookTwo);

            List<BookViewModel> books = this.favoriteBooksService
                                            .GetUserFavoriteBooks("978bc407-6236-46ac-80e1-3b3614a8dd05")
                                            .ToList();
            string firstBookTitle = books[0].Title;
            string secondBookTitle = books[1].Title;

            Assert.Equal(2, books.Count);
            Assert.Equal("First book title", firstBookTitle);
            Assert.Equal("Second book title", secondBookTitle);
        }
    }
}
