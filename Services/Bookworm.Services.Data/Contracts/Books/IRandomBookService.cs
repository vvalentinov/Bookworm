namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;

    public interface IRandomBookService
    {
        IEnumerable<BookDetailsViewModel> GenerateBooks(string category, int countBooks);
    }
}
