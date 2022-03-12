namespace Bookworm.Common
{
    public static class ErrorMessages
    {
        public const string BookTitleRequired = "Title field is required.";
        public const string BookTitleLenght = "Title must be between 5 and 250 characters long.";
        public const string BookDescriptionRequired = "Description field is required.";
        public const string BookDescriptionLength = "Description must be between 30 and 2500 characters long.";
        public const string BookLanguageRequired = "Language field is required.";
        public const string BookPublisherLenght = "Publisher must be between 2 and 100 characters long.";
        public const string BookPagesCountRequired = "Number of pages field is required.";
        public const string BookPagesCountRange = "The number of pages must be between 5 and 3000.";
        public const string BookPublishedYearRequired = "Year field is required.";
        public const string BookFileRequired = "Book file field is required.";
        public const string BookImageFileRequired = "Book image field is required.";
        public const string InvalidBookPublishedYear = "Invalid year value.";

        public const string QuoteContentRequired = "Content field is required!";
        public const string QuoteLength = "Quote length must be between 20 and 2000 characters!";

        public const string AuthorNameLength = "Author name must be between 2 and 50 characters!";

        public const string MovieTitleLenght = "Movie name must be between 3 and 150 characters!";
    }
}
