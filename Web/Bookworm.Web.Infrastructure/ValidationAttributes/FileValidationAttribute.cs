namespace Bookworm.Web.Infrastructure.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly int maxSize;
        private readonly string[] extensions;

        public FileValidationAttribute(int maxSize, string[] extensions)
        {
            this.maxSize = maxSize;
            this.extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is IFormFile file)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!this.extensions.Contains(fileExtension))
                {
                    string errMsg = string.Format(FileIncorrectExtensionError, string.Join(", ", this.extensions));
                    return new ValidationResult(errMsg);
                }

                if (file.Length > this.maxSize)
                {
                    string errMsg = fileExtension == BookFileAllowedExtension ?
                        BookInvalidPdfSizeError :
                        BookInvalidImageSizeError;

                    return new ValidationResult(errMsg);
                }
            }

            return ValidationResult.Success;
        }
    }
}
