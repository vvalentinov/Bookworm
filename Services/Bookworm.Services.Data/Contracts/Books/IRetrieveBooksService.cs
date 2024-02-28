namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        Task<BookListingViewModel> GetBooksAsync(int categoryId, int page);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page);

        Task<BookDetailsViewModel> GetBookDetails(int bookId, string userId);

        Task<UploadBookViewModel> GetEditBookAsync(int bookId);

        Task<List<BookDetailsViewModel>> GetPopularBooksAsync();

        Task<List<BookDetailsViewModel>> GetRecentBooksAsync();

        Task<List<BookDetailsViewModel>> GetUnapprovedBooksAsync();

        Task<List<BookDetailsViewModel>> GetApprovedBooksAsync();

        Task<List<BookDetailsViewModel>> GetDeletedBooksAsync();

        Task<int> GetUnapprovedBooksCountAsync();
    }
}
