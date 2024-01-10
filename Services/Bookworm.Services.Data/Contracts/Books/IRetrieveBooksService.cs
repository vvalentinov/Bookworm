namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<BookListingViewModel> GetBooksAsync(int categoryId, int page, int booksPerPage);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page, int booksPerPage);

        Task<BookViewModel> GetBookWithIdAsync(string bookId, string userId = null);

        Task<EditBookFormModel> GetEditBookAsync(string bookId);

        Task<IList<BookViewModel>> GetPopularBooksAsync(int count);

        Task<IList<BookViewModel>> GetRecentBooksAsync(int count);

        Task<List<BookViewModel>> GetUnapprovedBooksAsync();

        Task<List<BookViewModel>> GetApprovedBooksAsync();

        Task<List<BookViewModel>> GetDeletedBooksAsync();

        Task<int> GetUnapprovedBooksCountAsync();
    }
}
