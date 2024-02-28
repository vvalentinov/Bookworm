namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    public class BookListingViewModel : PagingViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; }
    }
}
