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

        QuoteListingViewModel GetAllQuotes(int page, int quotesPerPage);

        IEnumerable<T> GetAllUnapprovedQuotes<T>();

        T GetRandomQuote<T>();

        Task ApproveQuote(int id, string userId);

        UserQuotesViewModel GetUserQuotes(string userId);

        Task DeleteQuoteAsync(int quoteId);

        Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle);

        QuoteViewModel GetQuoteById(int quoteId);

        List<QuoteViewModel> GetUserApprovedQuotes(string userId);

        List<QuoteViewModel> GetUserUnapprovedQuotes(string userId);
    }
}
