namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.Books;

    public interface ISearchBooksService
    {
        Task<OperationResult<BookListingViewModel>> SearchBooksAsync(SearchBookInputModel model);

        Task<OperationResult<bool>> CheckIfBookWithTitleExistsAsync(
            string title,
            int? bookId = null);
    }
}
