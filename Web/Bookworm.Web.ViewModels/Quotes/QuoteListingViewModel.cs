namespace Bookworm.Web.ViewModels.Quotes
{
    using System;
    using System.Collections.Generic;

    public class QuoteListingViewModel
    {
        public IEnumerable<QuoteViewModel> Quotes { get; set; }

        public int PageNumber { get; set; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public int PreviousPageNumber => this.PageNumber - 1;

        public bool HasNextPage => this.PageNumber < this.PagesCount && this.QuotesCount > this.QuotesPerPage;

        public int NextPageNumber => this.PageNumber + 1;

        public int PagesCount => (int)Math.Ceiling((double)this.QuotesCount / this.QuotesPerPage);

        public int QuotesCount { get; set; }

        public int QuotesPerPage { get; set; }
    }
}
