namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IQuotesService
    {
        Task AddQuoteAsync(
            string content,
            string authorName,
            string bookTitle,
            string movieTitle,
            string userId);

        T GetRandomQuote<T>();
    }
}
