﻿namespace Bookworm.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Quotes.Models;

    public class IndexViewModel
    {
        public QuoteViewModel RandomQuote { get; set; }

        public IList<BookViewModel> RecentBooks { get; set; }

        public IList<BookViewModel> PopularBooks { get; set; }
    }
}
