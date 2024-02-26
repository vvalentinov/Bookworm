namespace Bookworm.Common.Books
{
    public static class BooksDataConstants
    {
        public const byte BookTitleMinLength = 2;
        public const byte BookTitleMaxLength = 100;

        public const byte BookDescriptionMinLength = 20;
        public const int BookDescriptionMaxLength = 3000;

        public const byte BookPagesCountMin = 5;
        public const int BookPagesCountMax = 3000;

        public const int BookPublishedYearMin = 1800;

        public const string BookFileAllowedExtension = ".pdf";

        public const string BookFileUploadPath = "Books/";

        public const string BookImageFileUploadPath = "BooksImages/";

        public const byte BooksCountOnHomePage = 8;

        public const byte BooksPerPage = 12;

        public const int BookPdfMaxSize = 15_000_000;

        public const int BookImageMaxSize = 5_000_000;
    }
}
