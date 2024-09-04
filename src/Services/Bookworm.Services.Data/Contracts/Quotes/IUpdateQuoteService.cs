namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.DTOs;

    public interface IUpdateQuoteService
    {
        Task<OperationResult> ApproveQuoteAsync(int quoteId);

        Task<OperationResult> DeleteQuoteAsync(
            int quoteId,
            string userId,
            bool isCurrUserAdmin = false);

        Task<OperationResult> UndeleteQuoteAsync(int quoteId);

        Task<OperationResult> UnapproveQuoteAsync(int quoteId);

        Task<OperationResult> EditQuoteAsync(
            QuoteDto quote,
            string userId);
    }
}
