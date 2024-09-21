namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;

    public class RetrieveBooksService : IRetrieveBooksService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICategoriesService categoriesService;

        private readonly IRepository<Comment> commentRepo;
        private readonly IRepository<FavoriteBook> favBookRepo;
        private readonly IRepository<AuthorBook> authorBookRepo;
        private readonly IDeletableEntityRepository<Book> bookRepo;

        public RetrieveBooksService(
            IUnitOfWork unitOfWork,
            ICategoriesService categoriesService)
        {
            this.unitOfWork = unitOfWork;

            this.categoriesService = categoriesService;

            this.favBookRepo = this.unitOfWork.GetRepository<FavoriteBook>();
            this.authorBookRepo = this.unitOfWork.GetRepository<AuthorBook>();
            this.bookRepo = this.unitOfWork.GetDeletableEntityRepository<Book>();
            this.commentRepo = this.unitOfWork.GetDeletableEntityRepository<Comment>();
        }

        public async Task<OperationResult<BookDetailsViewModel>> GetBookDetailsAsync(
            int bookId,
            string userId,
            bool isAdmin)
        {
            var bookViewModel = await this.bookRepo
                .AllAsNoTracking()
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.Publisher)
                .Select(book => new BookDetailsViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Year = book.Year,
                    PagesCount = book.PagesCount,
                    DownloadsCount = book.DownloadsCount,
                    ImageUrl = book.ImageUrl,
                    FileUrl = book.FileUrl,
                    CategoryName = book.Category.Name,
                    Language = book.Language.Name,
                    UserId = book.UserId,
                    PublisherName = book.Publisher.Name,
                    IsApproved = book.IsApproved,
                    Username = book.User.UserName,
                    IsUserBook = book.UserId == userId,
                })
                .FirstOrDefaultAsync(book => book.Id == bookId);

            if (bookViewModel == null)
            {
                return OperationResult.Fail<BookDetailsViewModel>(BookWrongIdError);
            }

            if (!isAdmin &&
                !bookViewModel.IsApproved &&
                !bookViewModel.IsUserBook)
            {
                return OperationResult.Fail<BookDetailsViewModel>(BookDetailsError);
            }

            var authors = await this.authorBookRepo
                .AllAsNoTracking()
                .Include(x => x.Author)
                .Where(x => x.BookId == bookId)
                .Select(x => x.Author.Name)
                .ToListAsync();

            bookViewModel.Authors = authors;

            if (bookViewModel.IsApproved)
            {
                var bookRatings = await this.bookRepo
                    .AllAsNoTracking()
                    .Where(x => x.Id == bookId)
                    .Include(x => x.Ratings)
                    .Select(x => x.Ratings)
                    .FirstAsync();

                bookViewModel.RatingsCount = bookRatings.Count;
                bookViewModel.RatingsAvg = bookRatings.Count > 0 ? bookRatings.Average(x => x.Value) : 0;

                var query = this.commentRepo
                    .AllAsNoTracking()
                    .Where(c => c.BookId == bookId);

                var commentsCount = await query.CountAsync();

                var comments = await query
                    .Include(c => c.User)
                    .Include(c => c.Votes)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        Content = c.Content,
                        NetWorth = c.NetWorth,
                        CreatedOn = c.CreatedOn,
                        UserUserName = c.User.UserName,
                        IsCommentOwner = c.UserId == userId,
                        UserVoteValue = c.Votes
                            .Where(v => v.CommentId == c.Id && v.UserId == userId)
                            .Select(v => (int)v.Value)
                            .FirstOrDefault(),
                    })
                    .OrderByDescending(c => c.CreatedOn)
                    .Take(5)
                    .ToListAsync();

                var commentListingModel = new CommentsListingViewModel
                {
                    Comments = comments,
                    ItemsPerPage = 5,
                    PageNumber = 1,
                    RecordsCount = commentsCount,
                };

                bookViewModel.CommentsListing = commentListingModel;

                if (userId != null)
                {
                    bookViewModel.IsFavorite = await this.favBookRepo
                        .AllAsNoTracking()
                        .AnyAsync(x => x.BookId == bookId && x.UserId == userId);

                    var userRating = bookRatings.FirstOrDefault(x => x.UserId == userId);
                    bookViewModel.UserRating = userRating == null ? 0 : userRating.Value;
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
                .Select(x => new BookViewModel
                {
                    Id = x.Book.Id,
                    Title = x.Book.Title,
                    ImageUrl = x.Book.ImageUrl,
                });

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
