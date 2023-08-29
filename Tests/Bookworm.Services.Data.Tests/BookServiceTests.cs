namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Web.ViewModels.Books;
    using Moq;
    using Xunit;

    public class BookServiceTests
    {
        private readonly IList<Book> books;
        private readonly IList<AuthorBook> authorBooks;
        private readonly IList<Author> authors;
        private readonly IList<Publisher> publishers;
        private readonly IList<Comment> comments;
        private readonly IList<Rating> ratings;
        private readonly IList<Vote> votes;
        private readonly IList<FavoriteBook> favoriteBooks;

        private readonly BooksService booksService;

        public BookServiceTests()
        {
            this.authorBooks = new List<AuthorBook>()
            {
                new AuthorBook()
                {
                    BookId = "8e5fca84-9b02-4f98-9ca1-9268f2bfb62d",
                    AuthorId = 2,
                },
            };

            this.authors = new List<Author>()
            {
                new Author()
                {
                    Name = "Some name",
                    Id = 2,
                },
            };

            this.publishers = new List<Publisher>()
            {
                new Publisher()
                {
                    Id = 7,
                    Name = "Some publisher name",
                },
            };

            this.comments = new List<Comment>();
            this.ratings = new List<Rating>()
            {
                new Rating()
                {
                    BookId = "8e5fca84-9b02-4f98-9ca1-9268f2bfb62d",
                    UserId = "e397ffe3-95a4-4b13-b9b7-9c84bafccc32",
                },
            };
            this.votes = new List<Vote>();

            this.favoriteBooks = new List<FavoriteBook>()
            {
                new FavoriteBook()
                {
                    BookId = "8e5fca84-9b02-4f98-9ca1-9268f2bfb62d",
                    UserId = "e397ffe3-95a4-4b13-b9b7-9c84bafccc32",
                },
            };

            this.books = new List<Book>()
            {
                new Book()
                {
                    Id = "77e6fd96-e081-441b-a349-1e6f00e8a5ca",
                    CategoryId = 2,
                    Title = "First book title",
                    Description = "First book description",
                    ImageUrl = "http://example.com/air",
                    FileUrl = "https://brother.example.org/",
                    UserId = "cc741abb-7aba-42eb-bc02-d64d931af949",
                    IsApproved = false,
                    DownloadsCount = 3,
                },
                new Book()
                {
                    Id = "8e5fca84-9b02-4f98-9ca1-9268f2bfb62d",
                    Title = "Second book title",
                    CategoryId = 5,
                    Description = "Second book description",
                    ImageUrl = "http://baseball.example.com/",
                    FileUrl = "https://act.example.com/",
                    UserId = "cc741abb-7aba-42eb-bc02-d64d931af949",
                    IsApproved = true,
                    DownloadsCount = 50,
                    CreatedOn = new System.DateTime(2022, 2, 12),
                    PublisherId = 7,
                    LanguageId = 8,
                },
                new Book()
                {
                    Id = "e26a0c37-8e3a-4a8c-a60f-2b41c0e32895",
                    Title = "Third book title",
                    CategoryId = 5,
                    Description = "Third book description",
                    ImageUrl = "https://www.example.com/base/bit.php",
                    FileUrl = "https://act.example.com/",
                    UserId = "a969ab08-af7a-47ea-bf45-fb6f5a809ffe",
                    IsApproved = true,
                    DownloadsCount = 30,
                    CreatedOn = new System.DateTime(2022, 5, 11),
                },
            };

            Mock<IRepository<AuthorBook>> mockAuthorBookRepo = new Mock<IRepository<AuthorBook>>();
            mockAuthorBookRepo.Setup(x => x.AllAsNoTracking()).Returns(this.authorBooks.AsQueryable());

            Mock<IDeletableEntityRepository<Author>> mockAuthorRepo = new Mock<IDeletableEntityRepository<Author>>();
            mockAuthorRepo.Setup(x => x.AllAsNoTracking()).Returns(this.authors.AsQueryable());

            Mock<IDeletableEntityRepository<Publisher>> mockPublisherRepo = new Mock<IDeletableEntityRepository<Publisher>>();
            mockPublisherRepo.Setup(x => x.AllAsNoTracking()).Returns(this.publishers.AsQueryable());

            Mock<IDeletableEntityRepository<Comment>> mockCommentRepo = new Mock<IDeletableEntityRepository<Comment>>();
            mockCommentRepo.Setup(x => x.AllAsNoTracking()).Returns(this.comments.AsQueryable());
            mockCommentRepo.Setup(x => x.All()).Returns(this.comments.AsQueryable());

            Mock<IRepository<Rating>> mockRatingRepo = new Mock<IRepository<Rating>>();
            mockRatingRepo.Setup(x => x.AllAsNoTracking()).Returns(this.ratings.AsQueryable());
            mockRatingRepo.Setup(x => x.All()).Returns(this.ratings.AsQueryable());

            Mock<IRepository<Vote>> mockVoteRepo = new Mock<IRepository<Vote>>();
            mockVoteRepo.Setup(x => x.AllAsNoTracking()).Returns(this.votes.AsQueryable());

            Mock<IRepository<FavoriteBook>> mockFavoriteBookRepo = new Mock<IRepository<FavoriteBook>>();
            mockFavoriteBookRepo.Setup(x => x.AllAsNoTracking()).Returns(this.favoriteBooks.AsQueryable());

            Mock<ILanguagesService> mockLanguagesService = new Mock<ILanguagesService>();

            Mock<ICategoriesService> mockCategoriesService = new Mock<ICategoriesService>();
            Mock<IDeletableEntityRepository<Book>> mockBookRepo = new Mock<IDeletableEntityRepository<Book>>();
            mockBookRepo.Setup(x => x.AllAsNoTracking()).Returns(this.books.AsQueryable());
            mockBookRepo.Setup(x => x.All()).Returns(this.books.AsQueryable());

            this.booksService = new BooksService(
                mockBookRepo.Object,
                mockAuthorBookRepo.Object,
                mockAuthorRepo.Object,
                mockPublisherRepo.Object,
                mockCommentRepo.Object,
                mockRatingRepo.Object,
                mockVoteRepo.Object,
                mockCategoriesService.Object,
                mockLanguagesService.Object,
                mockFavoriteBookRepo.Object);
        }

        [Fact]
        public void GetUnapprovedBooksShouldWorkCorrectly()
        {
            IEnumerable<BookViewModel> books = this.booksService.GetUnapprovedBooks();

            Assert.Single(books);
            Assert.Equal("77e6fd96-e081-441b-a349-1e6f00e8a5ca", books.SingleOrDefault().Id);
        }

        [Fact]
        public void GetUnapprovedBookWithIdShouldWorkCorrectly()
        {
            BookViewModel book = this.booksService.GetUnapprovedBookWithId("8e5fca84-9b02-4f98-9ca1-9268f2bfb62d");

            Assert.NotNull(book);
            Assert.Equal("https://act.example.com/", book.FileUrl);
            Assert.Equal("Second book description", book.Description);
            Assert.Equal("Second book title", book.Title);
            Assert.Equal("http://baseball.example.com/", book.ImageUrl);
        }

        [Fact]
        public void GetUserBooksShouldWorkCorrectly()
        {
            BookListingViewModel model = this.booksService.GetUserBooks("cc741abb-7aba-42eb-bc02-d64d931af949", 1, 12);
            var books = model.Books.ToList();
            Assert.Equal(2, books.Count);
            Assert.Equal("Second book title", books[0].Title);
            Assert.Equal("First book title", books[1].Title);
        }

        [Fact]
        public void GetRecentBooksShouldWorkCorrectly()
        {
            List<BookViewModel> books = this.booksService.GetRecentBooks(2).ToList();

            Assert.Equal(2, books.Count);
            Assert.Equal("https://www.example.com/base/bit.php", books[0].ImageUrl);
            Assert.Equal("http://baseball.example.com/", books[1].ImageUrl);
        }

        [Fact]
        public void GetPopularBooksShouldWorkCorrectly()
        {
            List<BookViewModel> books = this.booksService.GetPopularBooks(2).ToList();

            Assert.Equal("http://baseball.example.com/", books[0].ImageUrl);
            Assert.Equal("https://www.example.com/base/bit.php", books[1].ImageUrl);
        }

        [Fact]
        public void GetBooksShouldWorkCorrectly()
        {
            var model = this.booksService.GetBooks(5, 1, 2);
            Assert.Equal(2, model.BookCount);
        }

        [Fact]
        public void GetBookWithId()
        {
            var book = this.booksService.GetBookWithId("8e5fca84-9b02-4f98-9ca1-9268f2bfb62d", "e397ffe3-95a4-4b13-b9b7-9c84bafccc32");

            Assert.NotNull(book);
        }
    }
}
