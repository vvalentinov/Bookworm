namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;
    using static Bookworm.Services.Mapping.AutoMapperConfig;

    public class RetrieveBooksService : IRetrieveBooksService
    {
        private readonly IUsersService usersService;
        private readonly IRatingsService ratingsService;
        private readonly ICategoriesService categoriesService;
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<FavoriteBook> favBooksRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public RetrieveBooksService(
            IUsersService usersService,
            IRatingsService ratingsService,
            ICategoriesService categoriesService,
            IRepository<Comment> commentRepository,
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<FavoriteBook> favoriteBooksRepository)
        {
            this.usersService = usersService;
            this.bookRepository = bookRepository;
            this.ratingsService = ratingsService;
            this.commentRepository = commentRepository;
            this.categoriesService = categoriesService;
            this.favBooksRepository = favoriteBooksRepository;
        }

        public async Task<BookDetailsViewModel> GetBookDetailsAsync(int bookId, string userId, bool isAdmin)
        {
            var bookViewModel = await this.bookRepository
                .AllAsNoTracking()
                .Include(x => x.Publisher)
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .To<BookDetailsViewModel>()
                .FirstOrDefaultAsync(book => book.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            bookViewModel.IsUserBook = bookViewModel.UserId == userId;

            if (!bookViewModel.IsApproved && !isAdmin && !bookViewModel.IsUserBook)
            {
                throw new InvalidOperationException("You don't have permission to view this book!");
            }

            bookViewModel.Username = await this.usersService.GetUserNameByIdAsync(bookViewModel.UserId);

            if (bookViewModel.IsApproved)
            {
                bookViewModel.RatingsAvg = await this.ratingsService.GetAverageRatingAsync(bookId);
                bookViewModel.RatingsCount = await this.ratingsService.GetRatingsCountAsync(bookId);

                bookViewModel.Comments = await this.commentRepository
                    .AllAsNoTracking()
                    .Include(c => c.User)
                    .Include(c => c.Votes)
                    .Where(c => c.BookId == bookId)
                    .SelectComments(userId)
                    .OrderByDescending(c => c.CreatedOn)
                    .ToListAsync();

                if (userId != null)
                {
                    bookViewModel.IsFavorite = await this.favBooksRepository
                        .AllAsNoTracking()
                        .AnyAsync(x => x.BookId == bookId && x.UserId == userId);

                    bookViewModel.UserRating = await this.ratingsService.GetUserRatingAsync(bookId, userId);
                }
            }

            return bookViewModel;
        }

        public async Task<IEnumerable<BookViewModel>> GetRandomBooksAsync(int countBooks, int? categoryId)
        {
            var query = this.bookRepository.AllAsNoTracking();

            if (categoryId.HasValue)
            {
                if (!await this.categoriesService.CheckIfIdIsValidAsync((int)categoryId))
                {
                    throw new InvalidOperationException(CategoryNotFoundError);
                }

                query = query.Where(b => b.CategoryId == categoryId);
            }

            var books = await query
                .OrderBy(b => Guid.NewGuid())
                .Take(countBooks)
                .SelectBookViewModel()
                .ToListAsync();

            return books;
        }

        public async Task<UploadBookViewModel> GetEditBookAsync(int bookId, string userId)
        {
            var book = await this.bookRepository
                .AllAsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.AuthorsBooks)
                .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            if (book.UserId != userId)
            {
                throw new InvalidOperationException(BookEditError);
            }

            return MapperInstance.Map<UploadBookViewModel>(book);
        }

        public async Task<BookListingViewModel> GetBooksInCategoryAsync(string category, int page)
        {
            var categoryId = await this.categoriesService.GetCategoryIdAsync(category);

            var query = this.bookRepository
                                .AllAsNoTracking()
                                .Where(x => x.CategoryId == categoryId && x.IsApproved);

            var recordsCount = await query.CountAsync();

            var books = await query
                                .SelectBookViewModel()
                                .OrderByDescending(x => x.Id)
                                .Skip((page - 1) * BooksPerPage)
                                .Take(BooksPerPage)
                                .ToListAsync();

            return new BookListingViewModel
            {
                Books = books,
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
                RecordsCount = recordsCount,
            };
        }

        public async Task<BookListingViewModel> GetUserFavoriteBooksAsync(string userId, int page)
        {
            var itemsPerPage = 8;

            var query = this.favBooksRepository.AllAsNoTracking()
                .Where(fb => fb.UserId == userId)
                .Join(
                        this.bookRepository.AllAsNoTracking().Where(b => b.IsApproved),
                        fb => new { fb.BookId, fb.UserId },
                        b => new { BookId = b.Id, UserId = userId },
                        (fb, b) => new BookViewModel
                        {
                            Id = b.Id,
                            Title = b.Title,
                            ImageUrl = b.ImageUrl,
                        });

            var recordsCount = await query.CountAsync();

            var books = await query
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            return new BookListingViewModel
            {
                Books = books,
                PageNumber = page,
                RecordsCount = recordsCount,
                ItemsPerPage = itemsPerPage,
            };
        }

        public async Task<BookListingViewModel> GetUserBooksAsync(string userId, int page)
        {
            var query = this.bookRepository.AllAsNoTracking().Where(x => x.UserId == userId);

            var recordsCount = await query.CountAsync();

            var books = await query
                                .OrderByDescending(x => x.CreatedOn)
                                .SelectBookViewModel()
                                .Skip((page - 1) * BooksPerPage)
                                .Take(BooksPerPage)
                                .ToListAsync();

            return new BookListingViewModel
            {
                Books = books,
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
                RecordsCount = recordsCount,
            };
        }

        public async Task<IEnumerable<BookViewModel>> GetPopularBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved)
                    .OrderByDescending(x => x.DownloadsCount)
                    .Take(BooksCountOnHomePage)
                    .SelectBookViewModel()
                    .ToListAsync();

        public async Task<IEnumerable<BookViewModel>> GetRecentBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved)
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(BooksCountOnHomePage)
                    .SelectBookViewModel()
                    .ToListAsync();

        public async Task<IEnumerable<BookDetailsViewModel>> GetUnapprovedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => !book.IsApproved)
                    .SelectBookDetailsViewModel()
                    .ToListAsync();

        public async Task<int> GetUnapprovedBooksCountAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => !book.IsApproved)
                    .CountAsync();

        public async Task<IEnumerable<BookDetailsViewModel>> GetApprovedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTracking()
                    .Where(book => book.IsApproved)
                    .SelectBookDetailsViewModel()
                    .ToListAsync();

        public async Task<IEnumerable<BookDetailsViewModel>> GetDeletedBooksAsync()
            => await this.bookRepository
                    .AllAsNoTrackingWithDeleted()
                    .Where(book => book.IsDeleted)
                    .SelectBookDetailsViewModel()
                    .ToListAsync();

        public async Task<Book> GetBookWithIdAsync(int bookId, bool withTracking = false)
        {
            var bookQuery = withTracking ?
                this.bookRepository.All() :
                this.bookRepository.AllAsNoTracking();

            var book = await bookQuery.FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            return book;
        }

        public async Task<Book> GetDeletedBookWithIdAsync(int bookId, bool withTracking = false)
        {
            var bookQuery = withTracking ?
                this.bookRepository.AllWithDeleted() :
                this.bookRepository.AllAsNoTrackingWithDeleted();

            var book = await bookQuery.FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            return book;
        }
    }
}
