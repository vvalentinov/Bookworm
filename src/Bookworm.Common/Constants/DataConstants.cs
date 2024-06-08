namespace Bookworm.Common.Constants
{
    public static class DataConstants
    {
        public static class BookDataConstants
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
            public const byte BooksPerPage = 12;
            public const byte BooksCountOnHomePage = 8;
            public const int BookPdfMaxSize = 15_000_000;
            public const int BookImageMaxSize = 5_000_000;
            public const byte BookUploadPoints = 5;
        }

        public static class QuoteDataConstants
        {
            public const byte QuoteContentMinLength = 10;
            public const int QuoteContentMaxLength = 150;
            public const byte QuoteSourceMinLength = 2;
            public const byte QuoteSourceMaxLength = 150;
            public const byte QuotesPerPage = 6;
            public const byte QuoteUploadPoints = 2;
        }

        public static class PublisherDataConstants
        {
            public const byte PublisherNameMinLength = 2;
            public const byte PublisherNameMaxLength = 80;
        }

        public static class AuthorDataConstants
        {
            public const byte AuthorNameMinLength = 2;
            public const byte AuthorNameMaxLength = 50;
        }

        public static class CommentDataConstants
        {
            public const byte CommentContentMinLength = 20;
            public const int CommentContentMaxLength = 1000;
        }

        public static class RatingDataConstants
        {
            public const byte RatingValueMin = 1;
            public const byte RatingValueMax = 5;
        }

        public static class ApplicationUser
        {
            public const byte UserMaxDailyBookDownloadsCount = 10;
        }
    }
}
