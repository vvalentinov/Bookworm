namespace Bookworm.Common.Authors
{
    public static class AuthorsErrorMessagesConstants
    {
        public const string RequiredAuthorNameError = "Author name is required!";

        public const string AuthorNameLengthError = "Author name must be between {2} and {1} characters!";

        public const string AuthorDuplicatesError = "No author duplicates allowed!";

        public const string AuthorsCountError = "Authors count must be between 1 and 5!";
    }
}
