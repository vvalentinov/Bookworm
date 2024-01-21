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
    using Bookworm.Web.ViewModels.Authors;
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
            IEnumerable<UploadAuthorViewModel> authors,
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

            if (bookFile != null)
            {
                if (bookFile.Length == 0)
                {
                    throw new InvalidOperationException(BookPdfFileEmptyError);
                }

                if (bookFile.Length > 15_000_000)
                {
                    throw new InvalidOperationException(BookInvalidPdfSizeError);
                }

                string bookFileExtension = Path.GetExtension(bookFile.FileName);

                if (bookFileExtension != BookFileAllowedExtension)
                {
                    throw new InvalidOperationException(BookInvalidFileExtensionError);
                }
            }

            if (imageFile != null)
            {
                if (imageFile.Length == 0)
                {
                    throw new InvalidOperationException(BookImageFileEmptyError);
                }

                string bookImageExtension = Path.GetExtension(imageFile.FileName);

                if (this.permittedImageExtensions.Contains(bookImageExtension) == false)
                {
                    throw new InvalidOperationException(BookInvalidImageFileError);
                }
            }

            if (authors == null)
            {
                throw new InvalidOperationException(BookMissingAuthorsError);
            }

            bool hasDuplicates = authors
                .Select(x => x.Name)
                .GroupBy(author => author)
                .Any(group => group.Count() > 1);

            if (hasDuplicates)
            {
                throw new Exception("No author duplicates allowed!");
            }

            if (authors.Count() > 5)
            {
                throw new InvalidOperationException("Authors count must be between 1 and 5!");
            }
        }
    }
}
