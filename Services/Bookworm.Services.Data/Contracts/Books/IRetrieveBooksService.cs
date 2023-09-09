namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;

    public interface IRetrieveBooksService
    {
        BookListingViewModel GetBooks(int categoryId, int page, int booksPerPage);

        BookListingViewModel GetUserBooks(string userId, int page, int booksPerPage);

        BookViewModel GetBookWithId(string bookId, string userId = null);

        BookViewModel GetUnapprovedBookWithId(string bookId);

        IList<BookViewModel> GetPopularBooks(int count);

        IList<BookViewModel> GetRecentBooks(int count);

        IEnumerable<BookViewModel> GetUnapprovedBooks();

        IEnumerable<BookViewModel> GetApprovedBooks();

        IEnumerable<BookViewModel> GetDeletedBooks();
    }
}
