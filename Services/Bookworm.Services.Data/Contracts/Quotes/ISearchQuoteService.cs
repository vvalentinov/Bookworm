namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;

    using Bookworm.Common.Enums;
    using Bookworm.Web.ViewModels.Quotes;

    public interface ISearchQuoteService
    {
        List<QuoteViewModel> SearchQuoteByContent(
            string content,
            string userId,
            QuoteType? type = null);
    }
}
