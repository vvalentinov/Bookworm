namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    public interface ICheckIfQuoteExistsService
    {
        Task<bool> QuoteExists(string content);
    }
}
