namespace Bookworm.Common
{
    public static class DataConstants
    {
        public const byte BookTitleMinLength = 5;
        public const byte BookTitleMaxLength = 250;
        public const byte BookDescriptionMinLength = 40;
        public const int BookDescriptionMaxLength = 3000;
        public const byte BookPagesCountMin = 5;
        public const int BookPagesCountMax = 3000;
        public const int BookPublishedYearMin = 1800;

        public const byte AuthorNameMin = 2;
        public const byte AuthorNameMax = 50;

        public const byte BookPublisherMin = 2;
        public const byte BookPublisherMax = 100;

        public const byte QuoteMinLength = 10;
        public const int QuoteMaxLength = 350;

        public const byte MovieTitleMinLenght = 3;
        public const byte MovieTitleMaxLenght = 150;

        public const string BookFileAllowedExtension = ".pdf";
    }
}
