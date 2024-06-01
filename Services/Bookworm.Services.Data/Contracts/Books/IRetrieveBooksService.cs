namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<BookListingViewModel> GetBooksAsync(int categoryId, int page);

        Task<IEnumerable<BookViewModel>> GetRandomBooksAsync(int countBooks, int? categoryId);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page);

        Task<BookDetailsViewModel> GetBookDetailsAsync(int bookId, string userId);

        Task<UploadBookViewModel> GetEditBookAsync(int bookId, string userId);

        Task<List<BookViewModel>> GetPopularBooksAsync();

        Task<List<BookViewModel>> GetRecentBooksAsync();

        Task<List<BookDetailsViewModel>> GetUnapprovedBooksAsync();

        Task<List<BookDetailsViewModel>> GetApprovedBooksAsync();

        Task<List<BookDetailsViewModel>> GetDeletedBooksAsync();

        Task<int> GetUnapprovedBooksCountAsync();

        Task<Book> GetBookWithIdAsync(int bookId, bool withTracking = false);

        Task<Book> GetDeletedBookWithIdAsync(int bookId, bool withTracking = false);

        Task<BookListingViewModel> GetUserFavoriteBooksAsync(string userId, int page);
    }
}
