namespace Bookworm.Common.Books
{
    public static class BooksErrorMessagesConstants
    {
        public const string BookTitleRequiredError = "Book title is required!";
        public const string BookTitleLengthError = "Book title must be between {2} and {1} characters long!";

        public const string BookDescriptionRequiredError = "Book description is required!";
        public const string BookDescriptionLengthError = "Book description must be between {2} and {1} charcters long!";

        public const string BookPublisherLengthError = "Publisher must be between {2} and {1} characters long!";

        public const string BookPagesCountRangeError = "The number of pages must be between {1} and {2}!";

        public const string BookPublishedYearInvalidError = "Invalid year value!";

        public const string BookFileRequiredError = "PDF file is required!";
        public const string BookImageFileRequiredError = "Image file is required!";
        public const string BookPdfFileEmptyError = "PDF file is empty!";
        public const string BookInvalidPdfSizeError = "Book PDF file must not exceed 15 MB!";
        public const string BookImageFileEmptyError = "Book image file is empty!";
        public const string BookInvalidFileExtensionError = "Invalid book file extension. Allowed extension: .pdf!";
        public const string BookInvalidImageFileError = "Invalid image file extension. Allowed extensions: .jpg, .jpeg, .png!";

        public const string BookMissingAuthorsError = "You must add at least one author!";

        public const string BookWrongIdError = "No book with given id found!";

        public const string BookWithTitleExistsError = "Book with given title already exist!";

        public const string BookDeleteError = "You have to be either the book's owner or an administrator to delete it!";

        public const string BookEditError = "You have to be the book's owner to edit it!";
    }
}
