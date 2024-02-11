namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models.DTOs;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.ListingViewModels;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllByCriteriaAsync(GetQuotesApiDto getQuotesApiDto, string userId);

        Task<QuoteListingViewModel> GetAllApprovedAsync(string userId = null, int? page = null);

        Task<QuoteListingViewModel> GetAllUnapprovedAsync();

        Task<QuoteListingViewModel> GetAllDeletedAsync();

        Task<QuoteViewModel> GetRandomAsync();

        Task<int> GetUnapprovedCountAsync();

        Task<UserQuoteListingViewModel> GetAllUserQuotesAsync(string userId, int page);

        Task<EditQuoteInputModel> GetQuoteForEditAsync(int id, string userId);
    }
}
