namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;

    public interface IRandomBookService
    {
        IEnumerable<BookViewModel> GenerateBooks(string category, int countBooks);
    }
}
