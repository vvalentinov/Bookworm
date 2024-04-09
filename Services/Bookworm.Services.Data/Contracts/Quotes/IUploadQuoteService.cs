namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.DTOs;

    public interface IUploadQuoteService
    {
        Task UploadQuoteAsync(QuoteDto quoteDto, string userId);
    }
}
