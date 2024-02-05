namespace Bookworm.Web.ViewModels.Quotes.Models
{
    public class GetQuotesRequestModel
    {
        public string Type { get; set; }

        public string SortCriteria { get; set; }

        public string Content { get; set; }

        public string QuoteStatus { get; set; }

        public int Page { get; set; } = 1;

        public bool IsForUserQuotes { get; set; } = false;
    }
}
