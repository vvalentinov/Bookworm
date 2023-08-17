namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IQuotesService
    {
        Task AddGeneralQuoteAsync(
            string content,
            string authorName,
            string userId);

        Task AddMovieQuoteAsync(
            string content,
            string movieTitle,
            string userId);

        Task AddBookQuoteAsync(
            string content,
            string bookTitle,
            string author,
            string userId);

        string GetMovieQuoteImageUrl();

        string GetBookQuoteImageUrl();

        string GetGeneralQuoteImageUrl();

        Task<bool> QuoteExists(string content);

        Task<QuoteListingViewModel> GetAllQuotesAsync(string userId);

        Task<int> LikeQuoteAsync(int quoteId, string userId);

        Task<int> DislikeQuoteAsync(int quoteId, string userId);

        IEnumerable<T> GetAllUnapprovedQuotes<T>();

        T GetRandomQuote<T>();

        Task ApproveQuote(int id, string userId);

        Task<UserQuotesViewModel> GetUserQuotesAsync(string userId);

        Task DeleteQuoteAsync(int quoteId);

        Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle);

        QuoteViewModel GetQuoteById(int quoteId);

        List<QuoteViewModel> GetQuotesByType(string userId, QuoteType type);

        List<QuoteViewModel> SearchQuote(string content, string userId, QuoteType? type = null);
    }
}
