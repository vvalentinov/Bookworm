namespace Bookworm.Web.ViewModels.Quotes
{
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.DTOs;

    public class GetQuotesApiRequestModel : IMapTo<GetQuotesApiDto>
    {
        public string Type { get; set; }

        public int Page { get; set; } = 1;

        public string Content { get; set; }

        public string QuoteStatus { get; set; }

        public string SortCriteria { get; set; }

        public bool IsForUserQuotes { get; set; } = false;
    }
}
