namespace Bookworm.Common.Books
{
    public class BooksErrorMessagesConstants
    {
        public const string BookTitleRequiredError = "Title field is required!";
        public const string BookTitleLengthError = "Title must be between 5 and 250 characters long!";

        public const string BookDescriptionRequiredError = "Description field is required!";
        public const string BookDescriptionLengthError = "Description must be between 40 and 3000 charcters long!";

        public const string BookPublisherLengthError = "Publisher must be between 2 and 100 characters long!";

        public const string BookPagesCountRangeError = "Pages must be between 5 and 3000!";

        public const string BookPublishedYearInvalidError = "Invalid year value!";

        public const string BookFileRequiredError = "PDF file is required!";

        public const string BookImageFileRequiredError = "Image file is required!";

        public const string BookPdfFileEmptyError = "PDF file is empty!";
        public const string BookInvalidPdfSizeError = "Book PDF file must not exceed 50 MB!";

        public const string BookImageFileEmptyError = "Book image file is empty!";

        public const string BookFileInvalidExtensionError = "Must be in PDF format!";
        public const string BookInvalidImageFileError = "Valid formats are: JPG, JPEG and PNG!";

        public const string BookMissingAuthorsError = "You must add at least one author!";

        public const string ChangeBookFileNameError = "Try changing the PDF file name!";
        public const string ChangeImageFileNameError = "Try changing the image file name!";
    }
}
