namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class RandomBooksCountValidationAttribute : ValidationAttribute
    {
        private readonly List<int> countRandomBooks = new List<int> { 5, 10, 15, 20 };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int count && this.countRandomBooks.Contains(count))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(BookRandomCountError);
        }
    }
}
