namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class FileAllowedExtensionsValidationAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public FileAllowedExtensionsValidationAttribute(string[] extensions)
        {
            this.extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (this.extensions.Contains(fileExtension))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(string.Format(FileIncorrectExtensionError, string.Join(", ", this.extensions)));
        }
    }
}
