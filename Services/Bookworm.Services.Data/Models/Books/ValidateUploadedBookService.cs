namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class ValidateUploadedBookService : IValidateUploadedBookService
    {
        private readonly string[] permittedImageExtensions = { ".png", ".jpg", ".jpeg" };
        private readonly IBlobService blobService;

        public ValidateUploadedBookService(IBlobService blobService)
        {
            this.blobService = blobService;
        }

        public async void ValidateUploadedBook(
            IFormFile bookFile,
            IFormFile imageFile,
            IEnumerable<string> authors)
        {
            if (bookFile == null || bookFile.Length == 0)
            {
                throw new Exception(BookPdfFileEmptyError);
            }

            if (bookFile.Length > 50_000_000)
            {
                throw new Exception(BookInvalidPdfSizeError);
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                throw new Exception(BookImageFileEmptyError);
            }

            string bookFileExtension = Path.GetExtension(bookFile.FileName);
            string bookImageExtension = Path.GetExtension(imageFile.FileName);

            if (bookFileExtension != BookFileAllowedExtension)
            {
                throw new Exception(BookInvalidFileExtensionError);
            }

            if (this.permittedImageExtensions.Contains(bookImageExtension) == false)
            {
                throw new Exception(BookInvalidImageFileError);
            }

            if (authors == null)
            {
                throw new Exception(BookMissingAuthorsError);
            }

            if (await this.blobService.CheckIfBlobExistsAsync(bookFile.FileName))
            {
                throw new Exception(ChangeBookFileNameError);
            }

            if (await this.blobService.CheckIfBlobExistsAsync(imageFile.FileName))
            {
                throw new Exception(ChangeImageFileNameError);
            }
        }
    }
}
