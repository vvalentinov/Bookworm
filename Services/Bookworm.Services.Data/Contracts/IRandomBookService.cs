namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IRandomBookService
    {
        IEnumerable<SelectListItem> GetCategories();

        IEnumerable<BookViewModel> GenerateBooks(string category, int countBooks);
    }
}
