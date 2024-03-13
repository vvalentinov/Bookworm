namespace Bookworm.Web.ViewModels.Books.ListingViewModels
{
    using System.Collections.Generic;

    public class BookListingViewModel : PagingViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; }
    }
}
