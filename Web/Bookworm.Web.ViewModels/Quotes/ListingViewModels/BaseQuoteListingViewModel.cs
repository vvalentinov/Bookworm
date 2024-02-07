namespace Bookworm.Web.ViewModels.Quotes.ListingViewModels
{
    using System.Collections.Generic;
    using Bookworm.Web.ViewModels.Quotes;

    public abstract class BaseQuoteListingViewModel : PagingViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }
    }
}
