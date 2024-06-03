namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<BookListingViewModel> GetBooksInCategoryAsync(string category, int page);

        Task<IEnumerable<BookViewModel>> GetRandomBooksAsync(int countBooks, int? categoryId);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page);

        Task<BookDetailsViewModel> GetBookDetailsAsync(int bookId, string currentUserId, bool isAdmin);

        Task<UploadBookViewModel> GetEditBookAsync(int bookId, string userId);

        Task<IEnumerable<BookViewModel>> GetPopularBooksAsync();

        Task<IEnumerable<BookViewModel>> GetRecentBooksAsync();

        Task<IEnumerable<BookDetailsViewModel>> GetUnapprovedBooksAsync();

        Task<IEnumerable<BookDetailsViewModel>> GetApprovedBooksAsync();

        Task<IEnumerable<BookDetailsViewModel>> GetDeletedBooksAsync();

        Task<int> GetUnapprovedBooksCountAsync();

        Task<Book> GetBookWithIdAsync(int bookId, bool withTracking = false);

        Task<Book> GetDeletedBookWithIdAsync(int bookId, bool withTracking = false);

        Task<BookListingViewModel> GetUserFavoriteBooksAsync(string userId, int page);
    }
}
