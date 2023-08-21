namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<QuoteListingViewModel> GetAllQuotesAsync(string userId);

        Task<UserQuotesViewModel> GetUserQuotesAsync(string userId);

        QuoteListingViewModel GetAllApprovedQuotes();

        QuoteListingViewModel GetAllUnapprovedQuotes();

        QuoteListingViewModel GetAllDeletedQuotes();

        QuoteViewModel GetQuoteById(int quoteId);

        List<QuoteViewModel> GetQuotesByType(string userId, QuoteType type);

        T GetRandomQuote<T>();
    }
}
