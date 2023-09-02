namespace Bookworm.Common.Books
{
    public static class BooksDataConstants
    {
        public const byte BookTitleMinLength = 5;
        public const byte BookTitleMaxLength = 250;

        public const byte BookDescriptionMinLength = 40;
        public const int BookDescriptionMaxLength = 3000;

        public const byte BookPublisherMinLength = 2;
        public const byte BookPublisherMaxLength = 100;

        public const byte BookPagesCountMin = 5;
        public const int BookPagesCountMax = 3000;

        public const int BookPublishedYearMin = 1800;

        public const string BookFileAllowedExtension = ".pdf";

        public const string BookFileUploadPath = "Books/";
        public const string BookImageFileUploadPath = "BooksImages/";
    }
}
