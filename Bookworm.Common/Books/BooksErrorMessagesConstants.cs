namespace Bookworm.Common.Books
{
    public class BooksErrorMessagesConstants
    {
        public const string BookTitleRequiredError = "Title field is required!";
        public const string BookTitleLengthError = "Title must be between 5 and 250 characters long!";

        public const string BookDescriptionRequiredError = "Description field is required!";
        public const string BookDescriptionLengthError = "Description must be between 40 and 3000 charcters long!";

        public const string BookPublisherLengthError = "Publisher must be between 2 and 100 characters long!";

        public const string BookPagesCountRangeError = "The number of pages must be between 5 and 3000!";

        public const string BookPublishedYearInvalidError = "Invalid year value!";

        public const string BookFileRequiredError = "PDF file is required!";

        public const string BookImageFileRequiredError = "Image file is required!";

        public const string BookPdfFileEmptyError = "PDF file is empty!";
        public const string BookInvalidPdfSizeError = "Book PDF file must not exceed 50 MB!";

        public const string BookImageFileEmptyError = "Book image file is empty!";

        public const string BookInvalidFileExtensionError = "Invalid book file extension. Allowed extension: .pdf!";
        public const string BookInvalidImageFileError = "Invalid image file extension. Allowed extensions: .jpg, .jpeg, .png!";

        public const string BookMissingAuthorsError = "You must add at least one author!";

        public const string ChangeBookFileNameError = "Try changing the PDF file name!";
        public const string ChangeImageFileNameError = "Try changing the image file name!";
    }
}
