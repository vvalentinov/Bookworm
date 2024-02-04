namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllByTypeAsync(
            string sortCriteria,
            string userId,
            string type,
            string content,
            int page);

        Task<QuoteListingViewModel> GetAllApprovedAsync(
            string userId = null,
            int? page = null);

        Task<QuoteListingViewModel> GetAllUnapprovedAsync();

        Task<QuoteListingViewModel> GetAllDeletedAsync();

        Task<T> GetRandomAsync<T>();

        Task<int> GetUnapprovedCountAsync();
    }
}
