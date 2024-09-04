namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.DTOs;

    public interface IUploadQuoteService
    {
        Task<OperationResult> UploadQuoteAsync(
            QuoteDto quoteDto,
            string userId);
    }
}
