namespace Bookworm.Web.ViewModels.Quotes
{
    using System.Collections.Generic;

    public class QuoteListingViewModel : BaseListingViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }
    }
}
