namespace Bookworm.Web.Infrastructure.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    public class FileAllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public FileAllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                string extension = Path.GetExtension(file.FileName);
                if (!this.extensions.Contains(extension.ToLower()))
                {
                    var allowedExtensions = string.Join(", ", this.extensions);
                    return new ValidationResult($"File extension is incorrect! Valid extensions: {allowedExtensions}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
