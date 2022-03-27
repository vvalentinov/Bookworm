namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;

    public interface IBooksService
    {
        IEnumerable<T> GetBookCategories<T>();

        BookListingViewModel GetBooks(int categoryId, int page, int booksPerPage);

        BookViewModel GetBookWithId(string bookId, string userId = null);
    }
}
