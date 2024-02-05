namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Quotes.Contracts;
    using Bookworm.Web.ViewModels.Quotes.Models;

    public interface IRetrieveQuotesService
    {
        Task<T> GetByIdAsync<T>(int quoteId)
            where T : IQuoteViewModel, new();

        Task<T> GetAllByCriteriaAsync<T>(
            string sortCriteria,
            string userId,
            string type,
            string content,
            int page,
            string quoteStatus,
            bool isForUserQuotes)
            where T : BaseQuoteListingViewModel, new();

        Task<T> GetAllApprovedAsync<T>(
            string userId = null,
            int? page = null)
            where T : BaseQuoteListingViewModel, new();

        Task<T> GetAllUnapprovedAsync<T>()
            where T : BaseQuoteListingViewModel, new();

        Task<T> GetAllDeletedAsync<T>()
            where T : BaseQuoteListingViewModel, new();

        Task<T> GetRandomAsync<T>()
            where T : IQuoteViewModel;

        Task<int> GetUnapprovedCountAsync();

        Task<T> GetAllUserQuotesAsync<T>(string userId, int page)
            where T : UserQuoteListingViewModel, new();
    }
}
