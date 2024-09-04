namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.DTOs;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<OperationResult<QuoteViewModel>> GetByIdAsync(int quoteId);

        Task<OperationResult<QuoteListingViewModel>> GetAllByCriteriaAsync(
            string userId,
            GetQuotesApiDto getQuotesApiDto);

        Task<OperationResult<QuoteListingViewModel>> GetAllApprovedAsync(
            int? page = null,
            string userId = null);

        Task<OperationResult<QuoteListingViewModel>> GetAllUnapprovedAsync();

        Task<OperationResult<QuoteListingViewModel>> GetAllDeletedAsync();

        Task<OperationResult<QuoteViewModel>> GetRandomAsync();

        Task<OperationResult<int>> GetUnapprovedCountAsync();

        Task<OperationResult<QuoteListingViewModel>> GetAllUserQuotesAsync(
            string userId,
            int page);

        Task<OperationResult<UploadQuoteViewModel>> GetQuoteForEditAsync(
            int id,
            string userId);
    }
}
