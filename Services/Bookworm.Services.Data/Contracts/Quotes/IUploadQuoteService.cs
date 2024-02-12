namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public interface IUploadQuoteService
    {
        Task UploadQuoteAsync(QuoteDto quoteDto, string userId);
        //Task UploadGeneralQuoteAsync(
        //    string content,
        //    string authorName,
        //    string userId);

        //Task UploadMovieQuoteAsync(
        //    string content,
        //    string movieTitle,
        //    string userId);

        //Task UploadBookQuoteAsync(
        //    string content,
        //    string bookTitle,
        //    string author,
        //    string userId);
    }
}
