namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<QuoteListingViewModel> GetAllQuotesAsync(string userId);

        Task<UserQuotesViewModel> GetUserQuotesAsync(string userId);

        Task<QuoteListingViewModel> GetAllApprovedQuotesAsync();

        Task<QuoteListingViewModel> GetAllUnapprovedQuotesAsync();

        Task<QuoteListingViewModel> GetAllDeletedQuotesAsync();

        Task<QuoteViewModel> GetQuoteByIdAsync(int quoteId);

        Task<List<QuoteViewModel>> GetQuotesByTypeAsync(string userId, QuoteType type);

        Task<T> GetRandomQuoteAsync<T>();

        Task<int> GetUnapprovedQuotesCountAsync();
    }
}
