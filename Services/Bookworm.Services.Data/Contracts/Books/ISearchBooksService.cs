namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Books.ListingViewModels;

    public interface ISearchBooksService
    {
        Task<BookListingViewModel> SearchBooks(SearchBookInputModel model);
    }
}
