namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models.Enums;

    public interface ISearchQuoteService
    {
        Task<List<T>> SearchUserQuotesByContentAndTypeAsync<T>(
            string content,
            string userId,
            QuoteType type);

        Task<List<T>> SearchUserQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchQuotesByContentAndTypeAsync<T>(string content, QuoteType type);

        Task<List<T>> SearchQuotesByContentAsync<T>(string content);

        Task<List<T>> SearchUserLikedQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchLikedQuotesByContentAsync<T>(string content);

        Task<List<T>> SearchUserApprovedQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchApprovedQuotesByContentAsync<T>(string content);

        Task<List<T>> SearchUserUnapprovedQuotesByContentAsync<T>(string content, string userId);

        Task<List<T>> SearchUnapprovedQuotesByContentAsync<T>(string content);
    }
}
