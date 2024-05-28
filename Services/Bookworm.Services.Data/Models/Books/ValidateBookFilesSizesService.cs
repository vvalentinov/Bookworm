namespace Bookworm.Services.Data.Models.Books
{
    using System;

    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class ValidateBookFilesSizesService : IValidateBookFilesSizesService
    {
        public void ValidateUploadedBookFileSizes(
            bool isForEdit,
            IFormFile bookFile,
            IFormFile imageFile)
        {
            if (isForEdit)
            {
                if (bookFile != null)
                {
                    CheckFileSize(bookFile, BookPdfMaxSize, isForPdf: true);
                }

                if (imageFile != null)
                {
                    CheckFileSize(imageFile, BookImageMaxSize, isForPdf: false);
                }
            }
            else
            {
                if (bookFile == null)
                {
                    throw new InvalidOperationException(BookFileRequiredError);
                }

                if (imageFile == null)
                {
                    throw new InvalidOperationException(BookImageFileRequiredError);
                }

                CheckFileSize(bookFile, BookPdfMaxSize, isForPdf: true);
                CheckFileSize(imageFile, BookImageMaxSize, isForPdf: false);
            }
        }

        private static void CheckFileSize(IFormFile file, int maxSize, bool isForPdf)
        {
            if (file.Length == 0)
            {
                throw new InvalidOperationException(isForPdf ? BookPdfFileEmptyError : BookImageFileEmptyError);
            }

            if (file.Length > maxSize)
            {
                throw new InvalidOperationException(isForPdf ? BookInvalidPdfSizeError : BookInvalidImageSizeError);
            }
        }
    }
}
