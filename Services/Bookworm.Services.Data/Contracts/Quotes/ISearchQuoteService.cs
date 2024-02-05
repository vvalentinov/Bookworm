namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models.Enums;
    using Bookworm.Web.ViewModels.Quotes.Models;

    public interface ISearchQuoteService
    {
        Task<List<QuoteViewModel>> SearchQuotesByContentAsync(string content, string userId);

        Task<List<QuoteViewModel>> SearchQuotesByContentAndTypeAsync(
            string content,
            QuoteType type,
            string userId);

        Task<List<QuoteViewModel>> SearchLikedQuotesByContentAsync(string content, string userId);

        Task<List<T>> SearchApprovedQuotesByContentAsync<T>(string content);

        Task<List<T>> SearchUnapprovedQuotesByContentAsync<T>(string content);
    }
}
