namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    public interface IUpdateQuoteService
    {
        Task ApproveQuoteAsync(int id, string userId);

        Task DeleteQuoteAsync(int quoteId);

        Task UndeleteQuoteAsync(int quoteId);

        Task UnapproveQuoteAsync(int quoteId);

        Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle);
    }
}
