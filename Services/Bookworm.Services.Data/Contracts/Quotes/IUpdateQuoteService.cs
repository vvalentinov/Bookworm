namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.DTOs;

    public interface IUpdateQuoteService
    {
        Task ApproveQuoteAsync(int quoteId, string userId);

        Task DeleteQuoteAsync(int quoteId, string userId);

        Task UndeleteQuoteAsync(int quoteId);

        Task UnapproveQuoteAsync(int quoteId);

        Task EditQuoteAsync(QuoteDto quote, string userId);
    }
}
