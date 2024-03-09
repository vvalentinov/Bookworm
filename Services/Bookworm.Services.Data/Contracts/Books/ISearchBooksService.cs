namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface ISearchBooksService
    {
        Task<BookListingViewModel> SearchBooks(SearchBookInputModel model);
    }
}
