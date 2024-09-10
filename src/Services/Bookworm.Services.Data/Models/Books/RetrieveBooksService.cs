namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;

    public class RetrieveBooksService : IRetrieveBooksService
    {
        private readonly IRatingsService ratingsService;
        private readonly ICategoriesService categoriesService;

        private readonly IRepository<Comment> commentRepo;
        private readonly IRepository<FavoriteBook> favBookRepo;
        private readonly IDeletableEntityRepository<Book> bookRepo;

        public RetrieveBooksService(
            IRatingsService ratingsService,
            ICategoriesService categoriesService,
            IRepository<Comment> commentRepo,
            IDeletableEntityRepository<Book> bookRepo,
            IRepository<FavoriteBook> favBookRepo)
        {
            this.ratingsService = ratingsService;
            this.categoriesService = categoriesService;

            this.bookRepo = bookRepo;
            this.commentRepo = commentRepo;
            this.favBookRepo = favBookRepo;
        }

        public async Task<OperationResult<BookDetailsViewModel>> GetBookDetailsAsync(
            int bookId,
            string userId,
            bool isAdmin)
        {
            var bookViewModel = await this.bookRepo
                .AllAsNoTracking()
                .Include(x => x.Publisher)
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .ToBookDetailsViewModel(userId)
                .FirstOrDefaultAsync(book => book.Id == bookId);

            if (bookViewModel == null)
            {
                return OperationResult.Fail<BookDetailsViewModel>(BookWrongIdError);
            }

            if (!isAdmin && !bookViewModel.IsApproved && !bookViewModel.IsUserBook)
            {
                return OperationResult.Fail<BookDetailsViewModel>(BookDetailsError);
            }

            if (bookViewModel.IsApproved)
            {
                var getAvgRatingResult = await this.ratingsService.GetAverageRatingAsync(bookId);

                if (getAvgRatingResult.IsFailure)
                {
                    return OperationResult.Fail<BookDetailsViewModel>(getAvgRatingResult.ErrorMessage);
                }

                var getRatingsCountResult = await this.ratingsService.GetRatingsCountAsync(bookId);

                if (getRatingsCountResult.IsFailure)
                {
                    return OperationResult.Fail<BookDetailsViewModel>(getRatingsCountResult.ErrorMessage);
                }

                bookViewModel.RatingsAvg = getAvgRatingResult.Data;
                bookViewModel.RatingsCount = getRatingsCountResult.Data;

                bookViewModel.Comments = await this.commentRepo
                    .AllAsNoTracking()
                    .Include(c => c.User)
                    .Include(c => c.Votes)
                    .Where(c => c.BookId == bookId)
                    .SelectComments(userId)
                    .OrderByDescending(c => c.CreatedOn)
                    .ToListAsync();

                if (userId != null)
                {
                    bookViewModel.IsFavorite = await this.favBookRepo
                        .AllAsNoTracking()
                        .AnyAsync(x => x.BookId == bookId && x.UserId == userId);

                    var getUserRatingResult = await this.ratingsService.GetUserRatingAsync(bookId, userId);

                    if (getUserRatingResult.IsFailure)
                    {
                        return OperationResult.Fail<BookDetailsViewModel>(getUserRatingResult.ErrorMessage);
                    }

                    bookViewModel.UserRating = getUserRatingResult.Data;
                }
            }

            return OperationResult.Ok(bookViewModel);
        }

        public async Task<OperationResult<IEnumerable<BookViewModel>>> GetRandomBooksAsync(
            int countBooks,
            int? categoryId)
        {
            var query = this.bookRepo
                .AllAsNoTracking()
                .Where(x => x.IsApproved);

            if (categoryId.HasValue)
            {
                var isIdValid = (await this.categoriesService
                    .CheckIfIdIsValidAsync((int)categoryId)).Data;

                if (!isIdValid)
                {
                    return OperationResult.Fail<IEnumerable<BookViewModel>>(CategoryNotFoundError);
                }

                query = query.Where(b => b.CategoryId == categoryId);
            }

            var books = await query
                .OrderBy(b => Guid.NewGuid())
                .Take(countBooks)
                .SelectBookViewModel()
                .ToListAsync();

            return OperationResult.Ok(books);
        }

        public async Task<OperationResult<UploadBookViewModel>> GetEditBookAsync(
            int bookId,
            string userId)
        {
            var book = await this.bookRepo
                .AllAsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.AuthorsBooks)
                .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return OperationResult.Fail<UploadBookViewModel>(BookWrongIdError);
            }

            if (book.UserId != userId)
            {
                return OperationResult.Fail<UploadBookViewModel>(BookEditError);
            }

            var model = UploadBookViewModel.MapFromBook(book);

            return OperationResult.Ok(model);
        }

        public async Task<OperationResult<BookListingViewModel>> GetBooksInCategoryAsync(
            string category,
            int page)
        {
            var categoryIdResult = await this.categoriesService.GetCategoryIdAsync(category);

            if (categoryIdResult.IsFailure)
            {
                return OperationResult.Fail<BookListingViewModel>(categoryIdResult.ErrorMessage);
            }

            var categoryId = categoryIdResult.Data;

            var query = this.bookRepo
                .AllAsNoTracking()
                .Where(x => x.IsApproved && x.CategoryId == categoryId);

            var recordsCount = await query.CountAsync();

            var books = await query
                .SelectBookViewModel()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * BooksPerPage)
                .Take(BooksPerPage)
                .ToListAsync();

            var data = new BookListingViewModel
            {
                Books = books,
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
                RecordsCount = recordsCount,
            };

            return OperationResult.Ok(data);
        }

        public async Task<OperationResult<BookListingViewModel>> GetUserFavoriteBooksAsync(
            string userId,
            int page)
        {
            var itemsPerPage = 8;

            var query = this.favBookRepo
                .AllAsNoTracking()
                .Include(x => x.Book)
                .Where(x => x.UserId == userId)
                .SelectBookViewModel();

            var recordsCount = await query.CountAsync();

            var books = await query
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var data = new BookListingViewModel
            {
                Books = books,
                PageNumber = page,
                RecordsCount = recordsCount,
                ItemsPerPage = itemsPerPage,
            };

            return OperationResult.Ok(data);
        }

        public async Task<OperationResult<BookListingViewModel>> GetUserBooksAsync(
            string userId,
            int page)
        {
            var query = this.bookRepo
                .AllAsNoTracking()
                .Where(x => x.UserId == userId);

            var recordsCount = await query.CountAsync();

            var books = await query
                .OrderByDescending(x => x.CreatedOn)
                .SelectBookViewModel()
                .Skip((page - 1) * BooksPerPage)
                .Take(BooksPerPage)
                .ToListAsync();

            var model = new BookListingViewModel
            {
                Books = books,
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
                RecordsCount = recordsCount,
            };

            return OperationResult.Ok(model);
        }

        public async Task<OperationResult<IEnumerable<BookViewModel>>> GetPopularBooksAsync()
        {
            var books = await this.bookRepo
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.DownloadsCount)
                .Take(BooksCountOnHomePage)
                .SelectBookViewModel()
                .ToListAsync();

            return OperationResult.Ok(books);
        }

        public async Task<OperationResult<IEnumerable<BookViewModel>>> GetRecentBooksAsync()
        {
            var books = await this.bookRepo
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.CreatedOn)
                .Take(BooksCountOnHomePage)
                .SelectBookViewModel()
                .ToListAsync();

            return OperationResult.Ok(books);
        }

        public async Task<OperationResult<IEnumerable<BookDetailsViewModel>>> GetUnapprovedBooksAsync()
        {
            var books = await this.bookRepo
                .AllAsNoTracking()
                .Where(book => !book.IsApproved)
                .SelectBookDetailsViewModel()
                .ToListAsync();

            return OperationResult.Ok(books);
        }

        public async Task<OperationResult<int>> GetUnapprovedBooksCountAsync()
        {
            var count = await this.bookRepo
                .AllAsNoTracking()
                .Where(book => !book.IsApproved)
                .CountAsync();

            return OperationResult.Ok(count);
        }

        public async Task<OperationResult<IEnumerable<BookDetailsViewModel>>> GetApprovedBooksAsync()
        {
            var books = await this.bookRepo
                .AllAsNoTracking()
                .Where(book => book.IsApproved)
                .SelectBookDetailsViewModel()
                .ToListAsync();

            return OperationResult.Ok(books);
        }

        public async Task<OperationResult<IEnumerable<BookDetailsViewModel>>> GetDeletedBooksAsync()
        {
            var books = await this.bookRepo
                .AllAsNoTrackingWithDeleted()
                .Where(book => book.IsDeleted)
                .SelectBookDetailsViewModel()
                .ToListAsync();

            return OperationResult.Ok(books);
        }

        public async Task<OperationResult<Book>> GetBookWithIdAsync(int bookId)
        {
            var book = await this.bookRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookId);

            if (book == null)
            {
                return OperationResult.Fail<Book>(BookWrongIdError);
            }

            return OperationResult.Ok(book);
        }

        public async Task<OperationResult<Book>> GetDeletedBookWithIdAsync(int bookId)
        {
            var book = await this.bookRepo
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(x => x.IsDeleted && x.Id == bookId);

            if (book == null)
            {
                return OperationResult.Fail<Book>(BookWrongIdError);
            }

            return OperationResult.Ok(book);
        }
    }
}
