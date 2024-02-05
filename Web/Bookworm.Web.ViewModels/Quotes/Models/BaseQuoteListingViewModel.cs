namespace Bookworm.Web.ViewModels.Quotes.Models
{
    using System.Collections.Generic;

    public abstract class BaseQuoteListingViewModel : PagingViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }
    }
}
