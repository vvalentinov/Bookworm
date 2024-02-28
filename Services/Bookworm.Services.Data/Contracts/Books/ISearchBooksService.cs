namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface ISearchBooksService
    {
        Task<BookListingViewModel> SearchBooks(
            string input,
            int page,
            int categoryId);

        Task<BookListingViewModel> SearchUserBooks(
            string input,
            int page,
            string userId);
    }
}
