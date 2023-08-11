namespace Bookworm.Common
{
    public static class ErrorMessages
    {
        public const string BookTitleRequired = "Title field is required.";
        public const string BookTitleLength = "Title must be between 5 and 250 characters long.";
        public const string BookDescriptionRequired = "Description field is required.";
        public const string BookDescriptionLength = "Description must be between 30 and 2500 characters long.";
        public const string BookLanguageRequired = "Language field is required.";
        public const string BookPublisherLenght = "Publisher must be between 2 and 100 characters long.";
        public const string BookPagesCountRequired = "Number of pages field is required.";
        public const string BookPagesCountRange = "The number of pages must be between 5 and 3000.";
        public const string BookPublishedYearRequired = "Year field is required.";
        public const string BookPublisherLength = "Publisher must be between 2 and 100 characters!";
        public const string BookFileRequired = "Book file field is required.";
        public const string BookImageFileRequired = "Book image field is required.";
        public const string InvalidBookPublishedYear = "Invalid year value.";

        public const string QuoteContentRequired = "Content field is required!";
        public const string QuoteLength = "Quote length must be between 20 and 2000 characters!";

        public const string AuthorNameLength = "Author name must be between 2 and 50 characters!";
        public const string AuthorNameRequired = "Author name field is required!";

        public const string MovieTitleRequired = "Movie title field is required!";
        public const string MovieTitleLength = "Movie name must be between 3 and 150 characters!";
        public const string QuoteBookTitleRequired = "Book title field is required!";
        public const string QuoteBookTitleLength = "Book title must be between 3 and 150 characters!";

        public const string EmptyPdfField = "PDF file field is empty!";
        public const string InvalidPdfSize = "Book PDF file must not exceed 50 MB!";
        public const string EmptyImageField = "Image file field is empty!";
        public const string InvalidBookFileExtension = "Must be in PDF format!";
        public const string InvalidImageFileExtension = "Valid formats are: JPG, JPEG and PNG!";
        public const string EmptyAuthorsField = "You must add at least one author!";
        public const string InvalidAuthorNameLength = "You must add at least one author!";
        public const string ChangeBookFileName = "Please, try changing the PDF file name!";
        public const string ChangeImageFileName = "Please, try changing the image file name!";
    }
}
