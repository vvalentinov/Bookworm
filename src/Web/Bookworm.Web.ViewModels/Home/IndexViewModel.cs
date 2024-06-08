namespace Bookworm.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Quotes;

    public class IndexViewModel
    {
        public QuoteViewModel RandomQuote { get; set; }

        public IEnumerable<BookViewModel> RecentBooks { get; set; }

        public IEnumerable<BookViewModel> PopularBooks { get; set; }
    }
}
