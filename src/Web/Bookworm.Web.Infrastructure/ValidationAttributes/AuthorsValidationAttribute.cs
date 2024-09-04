namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.AuthorErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class AuthorsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is not ICollection authors ||
                authors.Count < 1 ||
                authors.Count > 5)
            {
                return new ValidationResult(BookAuthorsCountError);
            }

            var authorNames = new HashSet<string>();

            foreach (var authorName in authors)
            {
                if (authorName
                    .GetType()
                    .GetProperty("Name")?
                    .GetValue(authorName) is not string name)
                {
                    return new ValidationResult("Author name is invalid!");
                }

                if (!authorNames.Add(name))
                {
                    return new ValidationResult(AuthorDuplicatesError);
                }
            }

            return ValidationResult.Success;
        }
    }
}
