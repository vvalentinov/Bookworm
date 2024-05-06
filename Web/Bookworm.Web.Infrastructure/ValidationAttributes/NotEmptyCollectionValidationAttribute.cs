namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class NotEmptyCollectionValidationAttribute : ValidationAttribute
    {
        private readonly string collectionName;

        public NotEmptyCollectionValidationAttribute(string collectionName)
        {
            this.collectionName = collectionName;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is ICollection collection && collection.Count > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"The {this.collectionName} collection cannot be empty!");
        }
    }
}
