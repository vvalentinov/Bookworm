namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.EntityFrameworkCore;

    public class LanguagesService : ILanguagesService
    {
        private readonly IRepository<Language> languagesRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public LanguagesService(
            IRepository<Language> langugesRepository,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
            this.languagesRepository = langugesRepository;
        }

        public async Task<OperationResult<IEnumerable<LanguageViewModel>>> GetAllAsync()
        {
            var data = await this.languagesRepository
                    .AllAsNoTracking()
                    .Select(language => new LanguageViewModel
                    {
                        Id = language.Id,
                        Name = language.Name,
                    })
                    .ToListAsync();

            return OperationResult.Ok(data);
        }

        public async Task<OperationResult<IEnumerable<LanguageViewModel>>> GetAllInBookCategoryAsync(int categoryId)
        {
            var data = await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.IsApproved && x.CategoryId == categoryId)
                    .Select(x => new LanguageViewModel
                    {
                        Id = x.LanguageId,
                        Name = x.Language.Name,
                    })
                    .Distinct()
                    .ToListAsync();

            return OperationResult.Ok(data);
        }

        public async Task<OperationResult<IEnumerable<LanguageViewModel>>> GetAllInUserBooksAsync(string userId)
        {
            var data = await this.bookRepository
                    .AllAsNoTracking()
                    .Where(x => x.UserId == userId)
                    .Select(x => new LanguageViewModel
                    {
                        Id = x.LanguageId,
                        Name = x.Language.Name,
                    })
                    .Distinct()
                    .ToListAsync();

            return OperationResult.Ok(data);
        }

        public async Task<OperationResult<bool>> CheckIfIdIsValidAsync(int languageId)
        {
            var data = await this.languagesRepository
                .AllAsNoTracking()
                .AnyAsync(language => language.Id == languageId);

            return OperationResult.Ok(data);
        }
    }
}
