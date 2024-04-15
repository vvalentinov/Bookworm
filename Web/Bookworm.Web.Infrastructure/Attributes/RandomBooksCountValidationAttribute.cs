namespace Bookworm.Web.Infrastructure.Attributes
{
    using System.ComponentModel.DataAnnotations;

    public class RandomBooksCountValidationAttribute
        : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is int input && (
                input == 5 ||
                input == 10 ||
                input == 15 ||
                input == 20))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"The number of random books must be 5, 10, 15 or 20!");
        }
    }
}
