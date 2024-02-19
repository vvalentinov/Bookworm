namespace Bookworm.Web.Infrastructure.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PublishedYearValidationAttribute : ValidationAttribute
    {
        public PublishedYearValidationAttribute(int minYear)
        {
            this.MinYear = minYear;
        }

        public int MinYear { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                if (year >= this.MinYear && year <= DateTime.UtcNow.Year)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("Invalid year value!");
        }
    }
}
