namespace Bookworm.Web.ViewModels.Quotes
{
    using System.Collections.Generic;

    public class QuoteListingViewModel : PagingViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }
    }
}
