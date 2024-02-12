namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models.DTOs;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.ListingViewModels;
    using Bookworm.Web.ViewModels.Quotes.QuoteInputModels;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllByCriteriaAsync(GetQuotesApiDto getQuotesApiDto, string userId);

        Task<QuoteListingViewModel> GetAllApprovedAsync(string userId = null, int? page = null);

        Task<QuoteListingViewModel> GetAllUnapprovedAsync();

        Task<QuoteListingViewModel> GetAllDeletedAsync();

        Task<QuoteViewModel> GetRandomAsync();

        Task<int> GetUnapprovedCountAsync();

        Task<QuoteListingViewModel> GetAllUserQuotesAsync(string userId, int page);

        Task<(BaseQuoteInputModel, string)> GetQuoteForEditAsync(int id, string userId);
    }
}
