namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;

    public interface IRandomBookService
    {
        IEnumerable<BookViewModel> GenerateBooks(string category, int countBooks);
    }
}
