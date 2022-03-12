namespace Bookworm.Services.Data.Contracts
{
    public interface IQuotesService
    {
        T GetRandomQuote<T>();
    }
}
