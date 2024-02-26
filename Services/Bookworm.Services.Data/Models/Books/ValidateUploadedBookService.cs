namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Authors.AuthorsErrorMessagesConstants;
    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class ValidateUploadedBookService : IValidateUploadedBookService
    {
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
            bool isForEdit,
            int categoryId,
            int languageId,
            IFormFile bookFile,
            IFormFile imageFile,
            IEnumerable<UploadAuthorViewModel> authors)
        {
            if (!await this.categoryRepository.AllAsNoTracking().AnyAsync(c => c.Id == categoryId))
            {
                throw new InvalidOperationException("The given category doesn't exist!");
            }

            if (!await this.languageRepository.AllAsNoTracking().AnyAsync(l => l.Id == languageId))
            {
                throw new InvalidOperationException("The given language doesn't exist!");
            }

            if (isForEdit)
            {
                if (bookFile != null)
                {
                    CheckPdfFileSize(bookFile);
                }

                if (imageFile != null)
                {
                    CheckImageFileSize(imageFile);
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

                CheckPdfFileSize(bookFile);

                CheckImageFileSize(imageFile);
            }

            bool hasDuplicates = authors
                .Select(x => x.Name.Trim())
                .GroupBy(author => author)
                .Any(group => group.Count() > 1);

            if (hasDuplicates)
            {
                throw new Exception(AuthorDuplicatesError);
            }

            if (authors.Count() > 5)
            {
                throw new InvalidOperationException(AuthorsCountError);
            }
        }

        private static void CheckPdfFileSize(IFormFile bookFile)
        {
            if (bookFile.Length == 0)
            {
                throw new InvalidOperationException(BookPdfFileEmptyError);
            }

            if (bookFile.Length > BookPdfMaxSize)
            {
                throw new InvalidOperationException(BookInvalidPdfSizeError);
            }
        }

        private static void CheckImageFileSize(IFormFile imageFile)
        {
            if (imageFile.Length == 0)
            {
                throw new InvalidOperationException(BookImageFileEmptyError);
            }

            if (imageFile.Length > BookImageMaxSize)
            {
                throw new InvalidOperationException(BookInvalidImageSizeError);
            }
        }
    }
}
