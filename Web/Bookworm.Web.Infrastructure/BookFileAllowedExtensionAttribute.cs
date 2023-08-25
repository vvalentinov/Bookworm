namespace Bookworm.Web.Infrastructure
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    using Microsoft.AspNetCore.Http;

    public class BookFileAllowedExtensionAttribute : ValidationAttribute
    {
        private readonly string extension;

        public BookFileAllowedExtensionAttribute(string extension)
        {
            this.extension = extension;
        }

        public string GetErrorMessage()
        {
            return $"This file extension is not allowed!";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (this.extension != extension)
                {
                    return new ValidationResult(this.GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }
    }
}
