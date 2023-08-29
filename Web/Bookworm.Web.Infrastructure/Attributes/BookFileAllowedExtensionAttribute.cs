namespace Bookworm.Web.Infrastructure.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class BookFileAllowedExtensionAttribute : ValidationAttribute
    {
        private readonly string extension;

        public BookFileAllowedExtensionAttribute(string extension)
        {
            this.extension = extension;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
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
