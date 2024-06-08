namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<int> GetUnapprovedBooksCountAsync();

        Task<Book> GetBookWithIdAsync(int bookId);

        Task<Book> GetDeletedBookWithIdAsync(int bookId);

        Task<IEnumerable<BookViewModel>> GetRecentBooksAsync();

        Task<IEnumerable<BookViewModel>> GetPopularBooksAsync();

        Task<IEnumerable<BookDetailsViewModel>> GetDeletedBooksAsync();

        Task<IEnumerable<BookDetailsViewModel>> GetApprovedBooksAsync();

        Task<IEnumerable<BookDetailsViewModel>> GetUnapprovedBooksAsync();

        Task<UploadBookViewModel> GetEditBookAsync(int bookId, string userId);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page);

        Task<BookListingViewModel> GetUserFavoriteBooksAsync(string userId, int page);

        Task<BookListingViewModel> GetBooksInCategoryAsync(string category, int page);

        Task<IEnumerable<BookViewModel>> GetRandomBooksAsync(int countBooks, int? categoryId);

        Task<BookDetailsViewModel> GetBookDetailsAsync(int bookId, string currentUserId, bool isAdmin);
    }
}
