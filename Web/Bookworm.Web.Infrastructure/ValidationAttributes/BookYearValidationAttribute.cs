namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BookYearValidationAttribute : ValidationAttribute
    {
        private readonly int minYear;

        public BookYearValidationAttribute(int minYear)
        {
            this.minYear = minYear;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is int year)
            {
                if (year >= this.minYear && year <= DateTime.UtcNow.Year)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("Invalid year value!");
        }
    }
}
