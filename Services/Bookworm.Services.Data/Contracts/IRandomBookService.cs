namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;

    public interface IRandomBookService
    {
        IEnumerable<CategoryViewModel> GetCategories();

        IEnumerable<BookViewModel> GenerateBooks(string category, int countBooks);
    }
}
