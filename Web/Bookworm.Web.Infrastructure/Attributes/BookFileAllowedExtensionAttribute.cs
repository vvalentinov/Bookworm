namespace Bookworm.Web.Infrastructure.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class BookFileAllowedExtensionAttribute(string extension)
        : ValidationAttribute
    {
        private readonly string extension = extension;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                string extension = Path.GetExtension(file.FileName);
                if (this.extension != extension)
                {
                    return new ValidationResult(BookInvalidFileExtensionError);
                }
            }

            return ValidationResult.Success;
        }
    }
}
