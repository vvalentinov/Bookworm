namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    public class BookListingViewModel : PagingViewModel
    {
        public string CategoryName { get; set; }

        public IEnumerable<BookViewModel> Books { get; set; }
    }
}
