namespace Bookworm.Web.Infrastructure.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class ImageFileAllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public ImageFileAllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extensions.Contains(extension.ToLower()) == false)
                {
                    return new ValidationResult(BookInvalidImageFileError);
                }
            }

            return ValidationResult.Success;
        }
    }
}
