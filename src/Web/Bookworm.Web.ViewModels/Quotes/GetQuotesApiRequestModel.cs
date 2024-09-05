namespace Bookworm.Web.ViewModels.Quotes
{
    using Bookworm.Web.ViewModels.DTOs;

    public class GetQuotesApiRequestModel
    {
        public string Type { get; set; }

        public int Page { get; set; } = 1;

        public string Content { get; set; }

        public string QuoteStatus { get; set; }

        public string SortCriteria { get; set; }

        public bool IsForUserQuotes { get; set; } = false;

        public GetQuotesApiDto MapToGetQuotesApiDto()
        {
            return new GetQuotesApiDto
            {
                Content = this.Content,
                Type = this.Type,
                Page = this.Page,
                QuoteStatus = this.QuoteStatus,
                SortCriteria = this.SortCriteria,
                IsForUserQuotes = this.IsForUserQuotes,
            };
        }
    }
}
