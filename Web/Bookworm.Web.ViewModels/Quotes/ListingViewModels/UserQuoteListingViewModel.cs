namespace Bookworm.Web.ViewModels.Quotes.ListingViewModels
{
    public class UserQuoteListingViewModel : BaseQuoteListingViewModel
    {
        public int ApprovedQuotesCount { get; set; }

        public int UnapprovedQuotesCount { get; set; }
    }
}
