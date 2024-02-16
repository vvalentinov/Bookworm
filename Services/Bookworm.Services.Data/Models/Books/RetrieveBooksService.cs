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
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class RetrieveBooksService : IRetrieveBooksService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;
        private readonly IRepository<Author> authorRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly ICategoriesService categoriesService;
        private readonly IRatingsService ratingsService;

        public RetrieveBooksService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<AuthorBook> authorsBooksRepository,
            IRepository<Author> authorRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IRepository<Comment> commentRepository,
            ICategoriesService categoriesService,
            IRatingsService ratingsService)
        {
            this.bookRepository = bookRepository;
            this.authorsBooksRepository = authorsBooksRepository;
            this.authorRepository = authorRepository;
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
            this.categoriesService = categoriesService;
            this.ratingsService = ratingsService;
        }

        public async Task<BookViewModel> GetBookDetails(string bookId, string currentUserId)
        {
            var bookViewModel = await this.bookRepository
                .AllAsNoTracking()
                .Include(x => x.Publisher)
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .Select(x => new BookViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Year = x.Year,
                    UserId = x.UserId,
                    DownloadsCount = x.DownloadsCount,
                    PagesCount = x.PagesCount,
                    ImageUrl = x.ImageUrl,
                    IsUserBook = x.UserId == currentUserId,
                    FileUrl = x.FileUrl,
                    IsApproved = x.IsApproved,
                    PublisherName = x.Publisher.Name,
                    Language = x.Language.Name,
                    CategoryName = x.Category.Name,
                    Authors = x.AuthorsBooks.Select(ab => ab.Author.Name),
                }).FirstOrDefaultAsync(book => book.Id == bookId) ??
                throw new InvalidOperationException("No book with given id found!");

            var bookUser = await this.userRepository
                .AllAsNoTracking()
                .Select(x => new { x.Id, x.UserName })
                .FirstAsync(x => x.Id == bookViewModel.UserId);

            bookViewModel.Username = bookUser.UserName;

            if (bookViewModel.IsApproved)
            {
                bookViewModel.RatingsAvg = await this.ratingsService.GetAverageRatingAsync(bookId);
                bookViewModel.RatingsCount = await this.ratingsService.GetRatingsCountAsync(bookId);

                bookViewModel.Comments = await this.commentRepository
                    .AllAsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Votes)
                    .Where(x => x.BookId == bookId)
                    .Select(x => new CommentViewModel
                    {
                        Content = x.Content,
                        CreatedOn = x.CreatedOn,
                        IsCommentOwner = x.UserId == currentUserId,
                        NetWorth = x.NetWorth,
                        Id = x.Id,
                        UserId = x.UserId,
                        UserUserName = x.User.UserName,
                        UserVoteValue = x.Votes
                            .Where(v => v.CommentId == x.Id && v.UserId == currentUserId)
                            .Select(v => (int?)v.Value)
                            .FirstOrDefault() ?? 0,
                    }).ToListAsync();

                if (currentUserId != null)
                {
                    var currentUser = await this.userRepository
                        .AllAsNoTracking()
                        .Include(x => x.FavoriteBooks)
                        .FirstAsync(x => x.Id == currentUserId);

                    bookViewModel.IsFavorite = currentUser.FavoriteBooks.Any(x => x.BookId == bookId);

                    bookViewModel.UserRating = await this.ratingsService.GetUserRatingAsync(bookId, currentUserId);
                }
            }

            return bookViewModel;
        }

        public async Task<EditBookViewModel> GetEditBookAsync(string bookId)
         => await this.bookRepository
                .AllAsNoTracking()
                .Include(x => x.Publisher)
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .Select(x => new EditBookViewModel
                {
                    Id = x.Id,
                    Authors = x.AuthorsBooks
                        .Select(a => new UploadAuthorViewModel
                        {
                            Id = a.Author.Id,
                            Name = a.Author.Name,
                        }).ToList(),
                    CategoryId = x.CategoryId,
                    Description = x.Description,
                    LanguageId = x.LanguageId,
                    PagesCount = x.PagesCount,
                    PublishedYear = x.Year,
                    Publisher = x.Publisher.Name,
                    Title = x.Title,
                }).FirstOrDefaultAsync(book => book.Id == bookId) ??
                   throw new InvalidOperationException(BookWrongIdError);

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

        public async Task<List<BookViewModel>> GetPopularBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved)
                    .OrderByDescending(x => x.DownloadsCount)
                    .Take(BooksCountOnHomePage)
                    .Select(x => new BookViewModel { Id = x.Id, ImageUrl = x.ImageUrl })
                    .ToListAsync();

        public async Task<List<BookViewModel>> GetRecentBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved)
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(BooksCountOnHomePage)
                    .Select(x => new BookViewModel { Id = x.Id, ImageUrl = x.ImageUrl })
                    .ToListAsync();

        public async Task<List<BookViewModel>> GetUnapprovedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => book.IsApproved == false)
                    .Select(book => new BookViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        UserId = book.UserId,
                    }).ToListAsync();

        public async Task<int> GetUnapprovedBooksCountAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => book.IsApproved == false)
                    .CountAsync();

        public async Task<List<BookViewModel>> GetApprovedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => book.IsApproved)
                    .Select(book => new BookViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        UserId = book.UserId,
                    }).ToListAsync();

        public async Task<List<BookViewModel>> GetDeletedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTrackingWithDeleted()
                    .Where(book => book.IsDeleted)
                    .Select(book => new BookViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        UserId = book.UserId,
                    }).ToListAsync();
    }
}
