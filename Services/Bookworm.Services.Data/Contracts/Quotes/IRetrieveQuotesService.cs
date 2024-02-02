namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetQuoteByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllQuotesAsync(string userId);

        Task<List<QuoteViewModel>> GetAllQuotesByTypeAsync(
            string sortCriteria,
            string userId,
            string type,
            string content);

        Task<QuoteListingViewModel> GetAllApprovedQuotesAsync();

        Task<QuoteListingViewModel> GetAllUnapprovedQuotesAsync();

        Task<QuoteListingViewModel> GetAllDeletedQuotesAsync();

        Task<T> GetRandomQuoteAsync<T>();

        Task<int> GetUnapprovedQuotesCountAsync();

        Task<List<QuoteViewModel>> GetLikedQuotesAsync(
            string userId,
            string sortQuotesCriteria,
            string content);
    }
}
