namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class ValidateUploadedBookService : IValidateUploadedBookService
    {
        private readonly string[] permittedImageExtensions = { ".png", ".jpg", ".jpeg" };
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Language> languageRepository;

        public ValidateUploadedBookService(
            IRepository<Category> categoryRepository,
            IRepository<Language> languageRepository)
        {
            this.categoryRepository = categoryRepository;
            this.languageRepository = languageRepository;
        }

        public async Task ValidateUploadedBookAsync(
            IFormFile bookFile,
            IFormFile imageFile,
            IEnumerable<string> authors,
            int categoryId,
            int languageId)
        {
            if (await this.categoryRepository.AllAsNoTracking().AnyAsync(x => x.Id == categoryId) == false)
            {
                throw new InvalidOperationException("The given category doesn't exist!");
            }

            if (await this.languageRepository.AllAsNoTracking().AnyAsync(x => x.Id == languageId) == false)
            {
                throw new InvalidOperationException("The given language doesn't exist!");
            }

            if (bookFile == null || bookFile.Length == 0)
            {
                throw new InvalidOperationException(BookPdfFileEmptyError);
            }

            if (bookFile.Length > 50_000_000)
            {
                throw new InvalidOperationException(BookInvalidPdfSizeError);
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                throw new InvalidOperationException(BookImageFileEmptyError);
            }

            string bookFileExtension = Path.GetExtension(bookFile.FileName);
            string bookImageExtension = Path.GetExtension(imageFile.FileName);

            if (bookFileExtension != BookFileAllowedExtension)
            {
                throw new InvalidOperationException(BookInvalidFileExtensionError);
            }

            if (this.permittedImageExtensions.Contains(bookImageExtension) == false)
            {
                throw new InvalidOperationException(BookInvalidImageFileError);
            }

            if (authors == null)
            {
                throw new InvalidOperationException(BookMissingAuthorsError);
            }

            if (authors.Count() < 1 || authors.Count() > 5)
            {
                throw new InvalidOperationException("Authors must be between 1 and 5!");
            }
        }
    }
}
