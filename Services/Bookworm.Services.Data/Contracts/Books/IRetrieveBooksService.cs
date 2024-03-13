namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Books.ListingViewModels;

    public interface IRetrieveBooksService
    {
        Task<BookCategoryListingViewModel> GetBooksAsync(int categoryId, int page);

        Task<IEnumerable<BookViewModel>> GetRandomBooksAsync(int countBooks, int? categoryId);

        Task<BookListingViewModel> GetUserBooksAsync(string userId, int page);

        Task<BookDetailsViewModel> GetBookDetails(int bookId, string userId);

        Task<UploadBookViewModel> GetEditBookAsync(int bookId);

        Task<List<BookViewModel>> GetPopularBooksAsync();

        Task<List<BookViewModel>> GetRecentBooksAsync();

        Task<List<BookDetailsViewModel>> GetUnapprovedBooksAsync();

        Task<List<BookDetailsViewModel>> GetApprovedBooksAsync();

        Task<List<BookDetailsViewModel>> GetDeletedBooksAsync();

        Task<int> GetUnapprovedBooksCountAsync();
    }
}
