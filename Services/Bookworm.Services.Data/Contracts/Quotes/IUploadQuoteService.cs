namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public interface IUploadQuoteService
    {
        Task UploadQuoteAsync(QuoteDto quoteDto, string userId);
    }
}
