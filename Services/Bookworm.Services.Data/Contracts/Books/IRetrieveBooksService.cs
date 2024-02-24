namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<BookListingViewModel> GetBooksAsync(int categoryId, int page, int booksPerPage);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page, int booksPerPage);

        Task<BookViewModel> GetBookDetails(int bookId, string userId);

        Task<EditBookViewModel> GetEditBookAsync(int bookId);

        Task<List<BookViewModel>> GetPopularBooksAsync();

        Task<List<BookViewModel>> GetRecentBooksAsync();

        Task<List<BookViewModel>> GetUnapprovedBooksAsync();

        Task<List<BookViewModel>> GetApprovedBooksAsync();

        Task<List<BookViewModel>> GetDeletedBooksAsync();

        Task<int> GetUnapprovedBooksCountAsync();
    }
}
