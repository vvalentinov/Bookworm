namespace Bookworm.Common.Quotes
{
    public static class QuotesErrorMessagesConstants
    {
        public const string QuoteContentRequiredError = "Quote content is required!";
        public const string QuoteContentLengthError = "Quote content must be between 10 and 150 characters long!";

        public const string QuoteBookTitleRequiredError = "Quote book title field is required!";
        public const string QuoteBookTitleLengthError = "Quote book title field must be between 5 and 150 characters long!";

        public const string QuoteMovieTitleRequiredError = "Quote movie title field is required!";
        public const string QuoteMovieTitleLengthError = "Quote movie title field must be between 5 and 150 characters long!";

        public const string QuoteAuthorNameRequiredError = "Quote author field is required!";
        public const string QuoteAuthorNameLengthError = "Quote author name field must be between 5 and 50 characters long!";

        public const string QuoteExistsError = "This quote already exist! Try again!";
    }
}
