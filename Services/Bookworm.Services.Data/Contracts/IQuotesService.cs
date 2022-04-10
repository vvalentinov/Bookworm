namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Quotes;

    public interface IQuotesService
    {
        Task AddQuoteAsync(
            string content,
            string authorName,
            string bookTitle,
            string movieTitle,
            string userId);

        IEnumerable<T> GetAllQuotes<T>();

        IEnumerable<T> GetAllUnapprovedQuotes<T>();

        T GetRandomQuote<T>();

        Task ApproveQuote(int id, string userId);

        IEnumerable<QuoteViewModel> GetUserQuotes(string userId);

        Task DeleteQuoteAsync(int quoteId);

        Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle);

        QuoteViewModel GetQuoteById(int quoteId);
    }
}
