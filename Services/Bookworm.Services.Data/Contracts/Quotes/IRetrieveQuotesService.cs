namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models.DTOs;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllByCriteriaAsync(GetQuotesApiDto getQuotesApiDto, string userId);

        Task<QuoteListingViewModel> GetAllApprovedAsync(
            int? page = null,
            string userId = null,
            string paginationAction = null,
            string paginationController = null);

        Task<QuoteListingViewModel> GetAllUnapprovedAsync();

        Task<QuoteListingViewModel> GetAllDeletedAsync();

        Task<QuoteViewModel> GetRandomAsync();

        Task<int> GetUnapprovedCountAsync();

        Task<QuoteListingViewModel> GetAllUserQuotesAsync(string userId, int page);

        Task<UploadQuoteViewModel> GetQuoteForEditAsync(int id, string userId);
    }
}
