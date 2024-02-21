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
            }

            if (imageFile != null)
            {
                if (imageFile.Length == 0)
                {
                    throw new InvalidOperationException(BookImageFileEmptyError);
                }
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
    }
}
