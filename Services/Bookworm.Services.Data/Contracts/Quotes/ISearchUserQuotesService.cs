namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;

    public interface ISearchUserQuotesService
    {
        Task<List<T>> SearchUserQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchUserQuotesByContentAndTypeAsync<T>(string content, string userId, QuoteType type);

        Task<List<T>> SearchUserLikedQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchUserApprovedQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchUserUnapprovedQuotesByContentAsync<T>(string content, string userId);
    }
}
