namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Comments;

    public class BooksService : IBooksService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IDeletableEntityRepository<Comment> commentRepository;
        private readonly IRepository<Rating> ratingRepository;
        private readonly IRepository<Vote> voteRepository;
        private readonly ICategoriesService categoriesService;
        private readonly ILanguagesService languagesService;
        private readonly IRepository<FavoriteBook> favoriteBookRepository;

        public BooksService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<AuthorBook> authorsBooksRepository,
            IDeletableEntityRepository<Author> authorRepository,
            IDeletableEntityRepository<Publisher> publishersRepository,
            IDeletableEntityRepository<Comment> commentRepository,
            IRepository<Rating> ratingRepository,
            IRepository<Vote> voteRepository,
            ICategoriesService categoriesService,
            ILanguagesService languagesService,
            IRepository<FavoriteBook> favoriteBookRepository)
        {
            this.bookRepository = bookRepository;
            this.authorsBooksRepository = authorsBooksRepository;
            this.authorRepository = authorRepository;
            this.publishersRepository = publishersRepository;
            this.commentRepository = commentRepository;
            this.ratingRepository = ratingRepository;
            this.voteRepository = voteRepository;
            this.categoriesService = categoriesService;
            this.languagesService = languagesService;
            this.favoriteBookRepository = favoriteBookRepository;
        }

        public BookViewModel GetBookWithId(string bookId, string userId = null)
        {
            Book book = this.bookRepository.AllAsNoTracking().First(x => x.Id == bookId);

            bool isFavorite = this.favoriteBookRepository.AllAsNoTracking().Any(x => x.BookId == bookId && x.UserId == userId);

            string category = this.categoriesService.GetCategoryName(book.CategoryId);

            List<int> authorsIds = this.authorsBooksRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Select(x => x.AuthorId)
                .ToList();

            List<string> authors = this.authorRepository
                .AllAsNoTracking()
                .Where(x => authorsIds.Contains(x.Id))
                .Select(x => x.Name)
                .ToList();

            Publisher publisher = this.publishersRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Id == book.PublisherId);

            string publisherName = null;

            if (publisher != null)
            {
                publisherName = publisher.Name;
            }

            string language = this.languagesService.GetLanguageName(book.LanguageId);

            double votesAvg = 0;
            int votesCount = this.ratingRepository.AllAsNoTracking().Where(x => x.BookId == bookId).Count();

            if (votesCount > 0)
            {
                votesAvg = this.ratingRepository.All().Where(x => x.BookId == bookId).Average(x => x.Value);
            }

            List<CommentViewModel> comments = this.commentRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Select(x => new CommentViewModel()
                {
                    Content = x.Content,
                    CreatedOn = x.CreatedOn,
                    Id = x.Id,
                    UserId = x.UserId,
                    UserUserName = x.User.UserName,
                    DownVotesCount = this.voteRepository
                                         .AllAsNoTracking()
                                         .Where(v => v.CommentId == x.Id && v.Value == VoteValue.DownVote)
                                         .Count(),
                    UpVotesCount = this.voteRepository
                                         .AllAsNoTracking()
                                         .Where(v => v.CommentId == x.Id && v.Value == VoteValue.UpVote)
                                         .Count(),
                    UserVote = (int)this.voteRepository
                                        .AllAsNoTracking()
                                        .FirstOrDefault(v => v.UserId == userId && v.CommentId == x.Id).Value,
                })
                .ToList();

            return this.bookRepository
              .AllAsNoTracking()
              .Where(x => x.Id == bookId)
              .Select(x => new BookViewModel()
              {
                  Id = x.Id,
                  Title = x.Title,
                  Description = x.Description,
                  DownloadsCount = x.DownloadsCount,
                  FileUrl = x.FileUrl,
                  ImageUrl = x.ImageUrl,
                  Language = language,
                  PagesCount = x.PagesCount,
                  Year = x.Year,
                  Authors = authors,
                  PublisherName = publisherName,
                  CategoryName = category,
                  RatingsAvg = votesAvg,
                  RatingsCount = votesCount,
                  UserRating = this.ratingRepository.AllAsNoTracking().FirstOrDefault(x => x.BookId == bookId && x.UserId == userId).Value,
                  Comments = comments,
                  IsFavorite = isFavorite,
                  IsUserBook = userId != null && book.UserId == userId,
              }).FirstOrDefault();
        }

        public BookListingViewModel GetBooks(int categoryId, int page, int booksPerPage)
        {
            return new BookListingViewModel()
            {
                CategoryName = this.categoriesService.GetCategoryName(categoryId),
                Books = this.bookRepository
                            .AllAsNoTracking()
                            .Where(x => x.CategoryId == categoryId && x.IsApproved == true)
                            .Select(x => new BookViewModel()
                            {
                                Id = x.Id,
                                Title = x.Title,
                                ImageUrl = x.ImageUrl,
                            })
                            .Skip((page - 1) * booksPerPage)
                            .Take(booksPerPage)
                            .OrderByDescending(x => x.Id)
                            .ToList(),
                PageNumber = page,
                BookCount = this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.CategoryId == categoryId && x.IsApproved == true)
                                .Count(),
                BooksPerPage = booksPerPage,
            };
        }

        public BookListingViewModel GetUserBooks(string userId, int page, int booksPerPage)
        {
            return new BookListingViewModel()
            {
                Books = this.bookRepository
                            .AllAsNoTracking()
                            .Where(x => x.UserId == userId)
                            .OrderByDescending(x => x.CreatedOn)
                            .Select(x => new BookViewModel()
                            {
                                Id = x.Id,
                                Title = x.Title,
                                ImageUrl = x.ImageUrl,
                            })
                            .Skip((page - 1) * booksPerPage)
                            .Take(booksPerPage)
                            .OrderByDescending(x => x.Id)
                            .ToList(),
                PageNumber = page,
                BookCount = this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.UserId == userId)
                                .Count(),
                BooksPerPage = booksPerPage,
            };
        }

        public IList<BookViewModel> GetPopularBooks(int count)
        {
            return this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved == true)
                .OrderByDescending(x => x.DownloadsCount)
                .Take(count)
                .Select(x => new BookViewModel()
                {
                    ImageUrl = x.ImageUrl,
                    Id = x.Id,
                })
                .ToList();
        }

        public IList<BookViewModel> GetRecentBooks(int count)
        {
            return this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved == true)
                .OrderByDescending(x => x.CreatedOn)
                .Take(count)
                .Select(x => new BookViewModel()
                {
                    ImageUrl = x.ImageUrl,
                    Id = x.Id,
                })
                .ToList();
        }

        public IEnumerable<BookViewModel> GetUnapprovedBooks()
        {
            return this.bookRepository
               .AllAsNoTracking()
               .Where(x => x.IsApproved == false)
               .Select(x => new BookViewModel()
               {
                   Id = x.Id,
                   UserId = x.UserId,
                   ImageUrl = x.ImageUrl,
                   Title = x.Title,
               }).ToList();
        }

        public BookViewModel GetUnapprovedBookWithId(string bookId)
        {
            Book book = this.bookRepository.All().First(x => x.Id == bookId);

            string category = this.categoriesService.GetCategoryName(book.CategoryId);

            List<int> authorsIds = this.authorsBooksRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Select(x => x.AuthorId)
                .ToList();

            List<string> authors = this.authorRepository
                .AllAsNoTracking()
                .Where(x => authorsIds.Contains(x.Id))
                .Select(x => x.Name)
                .ToList();

            Publisher publisher = this.publishersRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Id == book.PublisherId);

            string publisherName = null;

            if (publisher != null)
            {
                publisherName = publisher.Name;
            }

            string language = this.languagesService.GetLanguageName(book.LanguageId);

            return this.bookRepository
              .AllAsNoTracking()
              .Where(x => x.Id == bookId)
              .Select(x => new BookViewModel()
              {
                  Id = x.Id,
                  UserId = x.UserId,
                  Title = x.Title,
                  Description = x.Description,
                  FileUrl = x.FileUrl,
                  ImageUrl = x.ImageUrl,
                  Language = language,
                  PagesCount = x.PagesCount,
                  Year = x.Year,
                  Authors = authors,
                  PublisherName = publisherName,
                  CategoryName = category,
              }).FirstOrDefault();
        }
    }
}
