namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;

    public interface IBooksService
    {
        BookListingViewModel GetBooks(int categoryId, int page, int booksPerPage);

        IEnumerable<BookViewModel> GetUserBooks(string userId);

        BookViewModel GetBookWithId(string bookId, string userId = null);

        BookViewModel GetUnapprovedBookWithId(string bookId);

        IList<T> GetPopularBooks<T>(int count);

        IList<T> GetRecentBooks<T>(int count);

        IEnumerable<BookViewModel> GetUnapprovedBooks();
    }
}
