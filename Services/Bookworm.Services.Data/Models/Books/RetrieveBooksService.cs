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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;
    using static Bookworm.Common.GlobalConstants;

    public class RetrieveBooksService : IRetrieveBooksService
    {
        private readonly IRatingsService ratingsService;
        private readonly IRepository<Comment> commentRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public RetrieveBooksService(
            IRatingsService ratingsService,
            IRepository<Comment> commentRepository,
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.ratingsService = ratingsService;
            this.commentRepository = commentRepository;
            this.userManager = userManager;
            this.bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookViewModel>> GetRandomBooksAsync(int countBooks, int? categoryId)
        {
            var query = this.bookRepository.AllAsNoTracking();

            if (categoryId != null)
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }

            var books = await query
                .OrderBy(b => Guid.NewGuid())
                .Take(countBooks)
                .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                .ToListAsync();

            return books;
        }

        public async Task<BookDetailsViewModel> GetBookDetails(int bookId, string currentUserId)
        {
            var bookViewModel = await this.bookRepository
                .AllAsNoTracking()
                .Include(x => x.Publisher)
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .Select(x => new BookDetailsViewModel
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
                throw new InvalidOperationException(BookWrongIdError);

            var currentUser = await this.userManager
                        .Users
                        .Include(u => u.FavoriteBooks)
                        .FirstOrDefaultAsync(u => u.Id == currentUserId);

            var isUserAdmin = currentUser != null && await this.userManager.IsInRoleAsync(currentUser, AdministratorRoleName);

            if (!bookViewModel.IsApproved && !isUserAdmin && !bookViewModel.IsUserBook)
            {
                throw new InvalidOperationException("You don't have permission to view this book!");
            }

            var bookUser = await this.userManager.Users.FirstAsync(u => u.Id == bookViewModel.UserId);

            bookViewModel.Username = bookUser.UserName;

            if (bookViewModel.IsApproved)
            {
                bookViewModel.RatingsAvg = await this.ratingsService.GetAverageRatingAsync(bookId);

                bookViewModel.RatingsCount = await this.ratingsService.GetRatingsCountAsync(bookId);

                bookViewModel.Comments = await this.commentRepository
                    .AllAsNoTracking()
                    .Include(c => c.User)
                    .Include(c => c.Votes)
                    .Where(c => c.BookId == bookId)
                    .Select(c => new CommentViewModel
                    {
                        Content = c.Content,
                        CreatedOn = c.CreatedOn,
                        IsCommentOwner = c.UserId == currentUserId,
                        NetWorth = c.NetWorth,
                        Id = c.Id,
                        UserId = c.UserId,
                        UserUserName = c.User.UserName,
                        UserVoteValue = c.Votes
                            .Where(v => v.CommentId == c.Id && v.UserId == currentUserId)
                            .Select(v => (int)v.Value)
                            .FirstOrDefault(),
                    }).ToListAsync();

                if (currentUserId != null)
                {
                    bookViewModel.IsFavorite = currentUser.FavoriteBooks.Any(x => x.BookId == bookId);

                    bookViewModel.UserRating = await this.ratingsService.GetUserRatingAsync(bookId, currentUserId);
                }
            }

            return bookViewModel;
        }

        public async Task<UploadBookViewModel> GetEditBookAsync(int bookId)
            => await this.bookRepository
                .AllAsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.AuthorsBooks)
                .ThenInclude(b => b.Author)
                .Select(b => new UploadBookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishedYear = b.Year,
                    CategoryId = b.CategoryId,
                    LanguageId = b.LanguageId,
                    PagesCount = b.PagesCount,
                    Description = b.Description,
                    Publisher = b.Publisher.Name,
                    Authors = b.AuthorsBooks.Select(a => new UploadAuthorViewModel { Name = a.Author.Name }).ToList(),
                }).FirstOrDefaultAsync(b => b.Id == bookId) ?? throw new InvalidOperationException(BookWrongIdError);

        public async Task<BookListingViewModel> GetBooksAsync(int categoryId, int page)
            => new BookListingViewModel
            {
                Books = await this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.CategoryId == categoryId && x.IsApproved)
                                .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                                .OrderByDescending(x => x.Id)
                                .Skip((page - 1) * BooksPerPage)
                                .Take(BooksPerPage)
                                .ToListAsync(),
                PageNumber = page,
                RecordsCount = await this.bookRepository
                                    .AllAsNoTracking()
                                    .Where(x => x.CategoryId == categoryId && x.IsApproved)
                                    .CountAsync(),
                ItemsPerPage = BooksPerPage,
            };

        public async Task<BookListingViewModel> GetUserBooksAsync(string userId, int page)
            => new BookListingViewModel
            {
                Books = await this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.UserId == userId)
                                .OrderByDescending(x => x.CreatedOn)
                                .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                                .Skip((page - 1) * BooksPerPage)
                                .Take(BooksPerPage)
                                .ToListAsync(),
                RecordsCount = await this.bookRepository
                                    .AllAsNoTracking()
                                    .Where(x => x.UserId == userId)
                                    .CountAsync(),
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
            };

        public async Task<List<BookViewModel>> GetPopularBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved)
                    .OrderByDescending(x => x.DownloadsCount)
                    .Take(BooksCountOnHomePage)
                    .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                    .ToListAsync();

        public async Task<List<BookViewModel>> GetRecentBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved)
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(BooksCountOnHomePage)
                    .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                    .ToListAsync();

        public async Task<List<BookDetailsViewModel>> GetUnapprovedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => book.IsApproved == false)
                    .Select(book => new BookDetailsViewModel
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

        public async Task<List<BookDetailsViewModel>> GetApprovedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => book.IsApproved)
                    .Select(book => new BookDetailsViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        UserId = book.UserId,
                    }).ToListAsync();

        public async Task<List<BookDetailsViewModel>> GetDeletedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTrackingWithDeleted()
                    .Where(book => book.IsDeleted)
                    .Select(book => new BookDetailsViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        UserId = book.UserId,
                    }).ToListAsync();
    }
}
