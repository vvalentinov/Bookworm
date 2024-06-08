namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    public class BookListingViewModel : BaseListingViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; }
    }
}
