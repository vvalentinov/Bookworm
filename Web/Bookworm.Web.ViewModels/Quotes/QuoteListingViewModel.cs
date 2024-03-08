namespace Bookworm.Web.ViewModels.Quotes
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels;

    public class QuoteListingViewModel : PagingViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }
    }
}
