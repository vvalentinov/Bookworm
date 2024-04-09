namespace Bookworm.Web.DTOs
{
    public class GetQuotesApiDto
    {
        public string Type { get; set; }

        public string SortCriteria { get; set; }

        public string Content { get; set; }

        public string QuoteStatus { get; set; }

        public int Page { get; set; }

        public bool IsForUserQuotes { get; set; }
    }
}
