namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class BookYearValidationAttribute : ValidationAttribute
    {
        private readonly int minYear;

        public BookYearValidationAttribute(int minYear)
        {
            this.minYear = minYear;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int year && year >= this.minYear && year <= DateTime.UtcNow.Year)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(BookInvalidYearError);
        }
    }
}
