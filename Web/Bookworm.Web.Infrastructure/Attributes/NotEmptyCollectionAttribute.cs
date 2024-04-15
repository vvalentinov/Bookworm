namespace Bookworm.Web.Infrastructure.Attributes
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class NotEmptyCollectionAttribute
        : ValidationAttribute
    {
        private readonly string collectionName;

        public NotEmptyCollectionAttribute(string collectionName)
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
