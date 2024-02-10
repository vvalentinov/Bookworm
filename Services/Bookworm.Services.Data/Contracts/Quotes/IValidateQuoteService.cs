namespace Bookworm.Services.Data.Contracts.Quotes
{
    public interface IValidateQuoteService
    {
        void ValidateGeneralQuote(string content, string author);

        void ValidateBookQuote(string content, string bookTitle, string author);

        void ValidateMovieQuote(string content, string movieTitle);
    }
}
