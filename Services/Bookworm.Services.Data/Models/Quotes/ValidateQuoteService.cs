namespace Bookworm.Services.Data.Models.Quotes
{
    using System;

    using Bookworm.Services.Data.Contracts.Quotes;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class ValidateQuoteService : IValidateQuoteService
    {
        public void ValidateGeneralQuote(string content, string author)
        {
            ValidateQuoteContent(content);
            ValidateQuoteAuthor(author);
        }

        public void ValidateBookQuote(string content, string bookTitle, string author)
        {
            ValidateQuoteContent(content);
            ValidateQuoteAuthor(author);

            if (string.IsNullOrWhiteSpace(bookTitle))
            {
                throw new InvalidOperationException(QuoteBookTitleRequiredError);
            }

            if (bookTitle.Length < QuoteBookTitleMinLength || bookTitle.Length > QuoteBookTitleMaxLength)
            {
                throw new InvalidOperationException(QuoteBookTitleLengthError);
            }
        }

        public void ValidateMovieQuote(string content, string movieTitle)
        {
            ValidateQuoteContent(content);

            if (string.IsNullOrWhiteSpace(movieTitle))
            {
                throw new InvalidOperationException(QuoteMovieTitleRequiredError);
            }

            if (movieTitle.Length < QuoteMovieTitleMinLength || movieTitle.Length > QuoteMovieTitleMaxLength)
            {
                throw new InvalidOperationException(QuoteMovieTitleLengthError);
            }
        }

        private static void ValidateQuoteContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(QuoteContentRequiredError);
            }

            if (content.Length < QuoteContentMinLength || content.Length > QuoteContentMaxLength)
            {
                throw new InvalidOperationException(QuoteContentLengthError);
            }
        }

        private static void ValidateQuoteAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new InvalidOperationException(QuoteAuthorNameRequiredError);
            }

            if (author.Length < QuoteAuthorNameMinLength || author.Length > QuoteAuthorNameMaxLength)
            {
                throw new InvalidOperationException(QuoteAuthorNameLengthError);
            }
        }
    }
}
