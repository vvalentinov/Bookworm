namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IBooksService
    {
        IEnumerable<SelectListItem> GetBookCategories();

        BookListingViewModel GetBooks(int categoryId, int page, int booksPerPage);

        BookViewModel GetBookWithId(string bookId, string userId = null);
    }
}
