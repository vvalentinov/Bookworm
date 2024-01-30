namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models.Enums;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveUserQuotesService
    {
        Task<UserQuoteListingViewModel> GetAllUserQuotesAsync(string userId);

        Task<List<T>> GetUserApprovedQuotesAsync<T>(string userId);

        Task<List<T>> GetUserUnapprovedQuotesAsync<T>(string userId);

        Task<List<T>> GetUserQuotesByTypeAsync<T>(string userId, QuoteType type);

        Task<List<T>> GetUserLikedQuotesAsync<T>(string userId);
    }
}
