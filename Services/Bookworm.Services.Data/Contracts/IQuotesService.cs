namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuotesService
    {
        Task AddQuoteAsync(
            string content,
            string authorName,
            string bookTitle,
            string movieTitle,
            string userId);

        IEnumerable<T> GetAllQuotes<T>();

        T GetRandomQuote<T>();
    }
}
