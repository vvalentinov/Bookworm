namespace Bookworm.Web.Infrastructure
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    public class ImageFileAllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public ImageFileAllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
        }

        public string GetErrorMessage()
        {
            return $"This file extension is not allowed!";
        }

        protected override ValidationResult IsValid(
                 object value,
                 ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (this.extensions.Contains(extension.ToLower()) == false)
                {
                    return new ValidationResult(this.GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }
    }
}
