namespace Bookworm.Web.ViewModels.Quotes.Models
{
    public class UserQuoteListingViewModel : BaseQuoteListingViewModel
    {
        public int ApprovedQuotesCount { get; set; }

        public int UnapprovedQuotesCount { get; set; }
    }
}
