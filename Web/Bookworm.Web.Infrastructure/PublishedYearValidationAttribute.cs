namespace Bookworm.Web.Infrastructure
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

        public override bool IsValid(object value)
        {
            if (value is int yearValue)
            {
                if (yearValue <= DateTime.UtcNow.Year
                    && yearValue >= this.MinYear)
                {
                    return true;
                }
            }

            return false;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
    }
}
