namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class BookAuthorsCountValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is ICollection collection && collection.Count >= 1 && collection.Count <= 5)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(BookAuthorsCountError);
        }
    }
}
