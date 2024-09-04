namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<OperationResult<int>> GetUnapprovedBooksCountAsync();

        Task<OperationResult<Book>> GetBookWithIdAsync(int bookId);

        Task<OperationResult<Book>> GetDeletedBookWithIdAsync(int bookId);

        Task<OperationResult<IEnumerable<BookViewModel>>> GetRecentBooksAsync();

        Task<OperationResult<IEnumerable<BookViewModel>>> GetPopularBooksAsync();

        Task<OperationResult<IEnumerable<BookDetailsViewModel>>> GetDeletedBooksAsync();

        Task<OperationResult<IEnumerable<BookDetailsViewModel>>> GetApprovedBooksAsync();

        Task<OperationResult<IEnumerable<BookDetailsViewModel>>> GetUnapprovedBooksAsync();

        Task<OperationResult<UploadBookViewModel>> GetEditBookAsync(
            int bookId,
            string userId);

        Task<OperationResult<BookListingViewModel>> GetUserBooksAsync(
            string userId,
            int page);

        Task<OperationResult<BookListingViewModel>> GetUserFavoriteBooksAsync(
            string userId,
            int page);

        Task<OperationResult<BookListingViewModel>> GetBooksInCategoryAsync(
            string category,
            int page);

        Task<OperationResult<IEnumerable<BookViewModel>>> GetRandomBooksAsync(
            int countBooks,
            int? categoryId);

        Task<OperationResult<BookDetailsViewModel>> GetBookDetailsAsync(
            int bookId,
            string userId,
            bool isAdmin);
    }
}
