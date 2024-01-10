namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    public class RetrieveBooksService : IRetrieveBooksService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<Vote> voteRepository;
        private readonly IRepository<FavoriteBook> favoriteBookRepository;
        private readonly ICategoriesService categoriesService;
        private readonly ILanguagesService languagesService;
        private readonly IRatingsService ratingsService;

        public RetrieveBooksService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<AuthorBook> authorsBooksRepository,
            IDeletableEntityRepository<Author> authorRepository,
            IDeletableEntityRepository<Publisher> publishersRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IRepository<Comment> commentRepository,
            IRepository<Vote> voteRepository,
            ICategoriesService categoriesService,
            ILanguagesService languagesService,
            IRepository<FavoriteBook> favoriteBookRepository,
            IRatingsService ratingsService)
        {
            this.bookRepository = bookRepository;
            this.authorsBooksRepository = authorsBooksRepository;
            this.authorRepository = authorRepository;
            this.publishersRepository = publishersRepository;
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
            this.voteRepository = voteRepository;
            this.categoriesService = categoriesService;
            this.languagesService = languagesService;
            this.favoriteBookRepository = favoriteBookRepository;
            this.ratingsService = ratingsService;
        }

        public async Task<BookViewModel> GetBookWithIdAsync(string bookId, string userId = null)
        {
            Book book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(book => book.Id == bookId) ??
                throw new InvalidOperationException("No book with given id found!");

            string categoryName = await this.categoriesService.GetCategoryNameAsync(book.CategoryId);

            List<int> authorsIds = await this.authorsBooksRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Select(x => x.AuthorId)
                .ToListAsync();

            List<string> authors = await this.authorRepository
                .AllAsNoTracking()
                .Where(author => authorsIds.Contains(author.Id))
                .Select(x => x.Name)
                .ToListAsync();

            Publisher publisher = await this.publishersRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(publisher => publisher.Id == book.PublisherId);

            string publisherName = publisher?.Name;

            string language = this.languagesService.GetLanguageName(book.LanguageId);

            double ratingsAvg = 0;
            int ratingsCount = await this.ratingsService.GetRatingsCountAsync(bookId);

            if (ratingsCount > 0)
            {
                ratingsAvg = await this.ratingsService.GetAverageRatingAsync(bookId);
            }

            BookViewModel model = new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                DownloadsCount = book.DownloadsCount,
                FileUrl = book.FileUrl,
                ImageUrl = book.ImageUrl,
                Language = language,
                PagesCount = book.PagesCount,
                Year = book.Year,
                Authors = authors,
                PublisherName = publisherName,
                CategoryName = categoryName,
                RatingsAvg = ratingsAvg,
                RatingsCount = ratingsCount,
            };

            List<CommentViewModel> comments = await this.commentRepository
                    .AllAsNoTracking()
                    .Where(comment => comment.BookId == bookId)
                    .OrderByDescending(comment => comment.CreatedOn)
                    .OrderByDescending(comment => comment.NetWorth)
                    .To<CommentViewModel>()
                    .ToListAsync();

            model.Comments = comments;

            if (userId != null)
            {
                ApplicationUser user = await this.userRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == book.UserId);
                model.Username = user.UserName;

                foreach (var comment in comments)
                {
                    Vote vote = await this.voteRepository
                        .AllAsNoTracking()
                        .FirstOrDefaultAsync(v => v.UserId == userId && v.CommentId == comment.Id);

                    comment.UserVoteValue = vote == null ? 0 : (int)vote.Value;
                }

                bool isFavorite = await this.favoriteBookRepository
                    .AllAsNoTracking()
                    .AnyAsync(x => x.BookId == bookId && x.UserId == userId);

                model.IsFavorite = isFavorite;
                model.IsUserBook = book.UserId == userId;

                int rating = await this.ratingsService.GetUserRatingAsync(bookId, userId);
                if (rating != 0)
                {
                    model.UserRating = rating;
                }
            }

            return model;
        }

        public async Task<EditBookFormModel> GetEditBookAsync(string bookId)
        {
            Book book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(book => book.Id == bookId) ??
                throw new InvalidOperationException("No book with given id found!");

            List<int> authorsIds = await this.authorsBooksRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Select(x => x.AuthorId)
                .ToListAsync();

            List<UploadAuthorViewModel> authors = await this.authorRepository
                .AllAsNoTracking()
                .Where(author => authorsIds.Contains(author.Id))
                .To<UploadAuthorViewModel>()
                .ToListAsync();

            Publisher publisher = await this.publishersRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(publisher => publisher.Id == book.PublisherId);

            string publisherName = publisher?.Name;

            string language = this.languagesService.GetLanguageName(book.LanguageId);

            EditBookFormModel model = new EditBookFormModel()
            {
                Id = book.Id,
                Authors = authors,
                Categories = this.categoriesService.GetAll<CategoryViewModel>(),
                Languages = this.languagesService.GetAllLanguages(),
                CategoryId = book.CategoryId,
                Description = book.Description,
                LanguageId = book.LanguageId,
                PagesCount = book.PagesCount,
                PublishedYear = book.Year,
                Publisher = publisherName,
                Title = book.Title,
                ImageUrl = book.ImageUrl,
            };

            return model;
        }

        public async Task<BookListingViewModel> GetBooksAsync(int categoryId, int page, int booksPerPage)
        {
            BookListingViewModel model = new BookListingViewModel()
            {
                CategoryName = await this.categoriesService.GetCategoryNameAsync(categoryId),
                Books = await this.bookRepository
                            .AllAsNoTracking()
                            .Where(x => x.CategoryId == categoryId && x.IsApproved)
                            .Select(x => new BookViewModel()
                            {
                                Id = x.Id,
                                Title = x.Title,
                                ImageUrl = x.ImageUrl,
                            })
                            .OrderByDescending(x => x.Id)
                            .Skip((page - 1) * booksPerPage)
                            .Take(booksPerPage)
                            .ToListAsync(),
                PageNumber = page,
                BookCount = await this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.CategoryId == categoryId && x.IsApproved)
                                .CountAsync(),
                BooksPerPage = booksPerPage,
            };

            return model;
        }

        public async Task<BookListingViewModel> GetUserBooksAsync(string userId, int page, int booksPerPage)
        {
            BookListingViewModel model = new BookListingViewModel()
            {
                Books = await this.bookRepository
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
                            .ToListAsync(),
                PageNumber = page,
                BookCount = await this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.UserId == userId)
                                .CountAsync(),
                BooksPerPage = booksPerPage,
            };

            return model;
        }

        public async Task<IList<BookViewModel>> GetPopularBooksAsync(int count)
        {
            List<BookViewModel> popularBooks = await this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.DownloadsCount)
                .Take(count)
                .Select(x => new BookViewModel()
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                })
                .ToListAsync();

            return popularBooks;
        }

        public async Task<IList<BookViewModel>> GetRecentBooksAsync(int count)
        {
            List<BookViewModel> recentBooks = await this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.CreatedOn)
                .Take(count)
                .Select(x => new BookViewModel()
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                })
                .ToListAsync();

            return recentBooks;
        }

        public async Task<List<BookViewModel>> GetUnapprovedBooksAsync()
        {
            List<BookViewModel> unapprovedBooks = await this.bookRepository
               .AllAsNoTracking()
               .Where(x => x.IsApproved == false)
               .Select(x => new BookViewModel()
               {
                   Id = x.Id,
                   UserId = x.UserId,
                   Title = x.Title,
               }).ToListAsync();

            return unapprovedBooks;
        }

        public async Task<int> GetUnapprovedBooksCountAsync()
        {
            int unapprovedBooksCount = await this.bookRepository
                .AllAsNoTracking()
                .Where(book => book.IsApproved == false)
                .CountAsync();

            return unapprovedBooksCount;
        }

        public async Task<List<BookViewModel>> GetApprovedBooksAsync()
        {
            List<BookViewModel> approvedBooks = await this.bookRepository
               .AllAsNoTracking()
               .Where(book => book.IsApproved)
               .Select(book => new BookViewModel()
               {
                   Id = book.Id,
                   UserId = book.UserId,
                   ImageUrl = book.ImageUrl,
                   Title = book.Title,
               }).ToListAsync();

            return approvedBooks;
        }

        public async Task<List<BookViewModel>> GetDeletedBooksAsync()
        {
            List<BookViewModel> deletedBooks = await this.bookRepository
              .AllAsNoTrackingWithDeleted()
              .Where(book => book.IsDeleted)
              .Select(book => new BookViewModel()
              {
                  Id = book.Id,
                  UserId = book.UserId,
                  ImageUrl = book.ImageUrl,
                  Title = book.Title,
              }).ToListAsync();

            return deletedBooks;
        }
    }
}
