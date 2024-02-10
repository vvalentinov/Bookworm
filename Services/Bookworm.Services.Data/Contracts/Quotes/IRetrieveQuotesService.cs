namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.ListingViewModels;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllByCriteriaAsync(
            string sortCriteria,
            string userId,
            string type,
            string content,
            int page,
            string quoteStatus,
            bool isForUserQuotes);

        Task<QuoteListingViewModel> GetAllApprovedAsync(
            string userId = null,
            int? page = null);

        Task<QuoteListingViewModel> GetAllUnapprovedAsync();

        Task<QuoteListingViewModel> GetAllDeletedAsync();

        Task<QuoteViewModel> GetRandomAsync();

        Task<int> GetUnapprovedCountAsync();

        Task<UserQuoteListingViewModel> GetAllUserQuotesAsync(string userId, int page);

        Task<EditQuoteViewModel> GetQuoteForEditAsync(int id, string userId);
    }
}
