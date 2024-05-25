namespace Bookworm.Common.Constants
{
    public static class ErrorMessagesConstants
    {
        public const string FieldRequiredError = "{0} field is required!";
        public const string FieldStringLengthError = "{0} field must be between {2} and {1} characters long!";
        public const string FieldRangeError = "{0} field's range must be between {1} and {2}!";
        public const string FieldInvalidError = "{0} field is invalid!";
        public const string FieldMaxLengthError = "{0} field must not exceed {1} characters!";

        public const string EmptyCollectionError = "{0} collection cannot be empty!";

        public const string FileIncorrectExtensionError = "File extension is incorrect! Valid extensions: {0}";

        public static class BookErrorMessagesConstants
        {
            public const string BookFileRequiredError = "PDF file is required!";
            public const string BookImageFileRequiredError = "Image file is required!";
            public const string BookPdfFileEmptyError = "PDF file is empty!";
            public const string BookInvalidPdfSizeError = "Book PDF file must not exceed 15 MB!";
            public const string BookInvalidImageSizeError = "Book image file must not exceed 5 MB!";
            public const string BookImageFileEmptyError = "Book image file is empty!";
            public const string BookMissingAuthorsError = "You must add at least one author!";
            public const string BookWrongIdError = "No book with given id found!";
            public const string BookWithTitleExistsError = "Book with given title already exist!";
            public const string BookDeleteError = "You have to be either the book's owner or an administrator to delete it!";
            public const string BookEditError = "You have to be the book's owner to edit it!";
            public const string BookNotApprovedError = "This book is not approved!";
            public const string BookInvalidYearError = "Invalid year value!";
            public const string BookRandomCountError = "The number of random books must be 5, 10, 15 or 20!";
            public const string BookAuthorsCountError = "Book author's count must be between 1 and 5!";
        }

        public static class CategoryErrorMessagesConstants
        {
            public const string CategoryNotFoundError = "The given category doesn't exist!";
        }

        public static class LanguageErrorMessagesConstants
        {
            public const string LanguageNotFoundError = "The given language doesn't exist!";
        }

        public static class QuoteErrorMessagesConstants
        {
            public const string QuoteExistsError = "This quote already exist! Try again!";
            public const string QuoteWrongIdError = "No quote with given id found!";
            public const string QuoteEditError = "You have to be the quote's creator to edit it!";
            public const string QuoteDeleteError = "You have to be either the quote's creator or an admin to delete it!";
            public const string QuoteInvalidTypeError = "Invalid quote type!";
            public const string QuoteApproveError = "You have to be an admin to approve a quote!";
        }

        public static class AuthorErrorMessagesConstants
        {
            public const string AuthorDuplicatesError = "No author duplicates allowed!";
            public const string AuthorsCountError = "Authors count must be between 1 and 5!";
        }

        public static class IdentityErrorMessagesConstants
        {
            public const string UserWrongIdError = "No user with given id found!";
            public const string ResetPasswordError = "There was a problem resetting your password! Please, try again!";
            public const string ConfirmEmailError = "There was a problem confirming your email! Please, try again!";
            public const string UserWithEmailError = "Email is already taken! Please, try again!";
            public const string LoginError = "Login failed! Try again!";
            public const string RegisterError = "Register failed! Try again!";
        }

        public static class UserErrorMessagesConstants
        {
            public const string UserDailyCountError = "You have reached your daily book download count of 10 books! Try again, tomorrow!";
        }

        public static class NotificationsMessagesConstants
        {
            public const string UnapprovedQuoteNotification = "Oops! Your quote: \"{0}\" has been unapproved by an administrator! You have lost {1} points!";
            public const string WrongNotificationIdError = "No notification found with given id!";
            public const string DeleteNotificationError = "Only the notification's owner can delete it!";
        }
    }
}
