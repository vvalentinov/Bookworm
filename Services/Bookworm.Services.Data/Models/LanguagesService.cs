namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
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
            this.languagesRepository = langugesRepository;
            this.bookRepository = bookRepository;
        }

        public async Task<List<LanguageViewModel>> GetAllAsync()
        {
            return await this.languagesRepository
                .AllAsNoTracking()
                .To<LanguageViewModel>()
                .ToListAsync();
        }

        public async Task<string> GetLanguageNameAsync(int languageId)
        {
            Language language = await this.languagesRepository
                .AllAsNoTracking()
                .FirstAsync(language => language.Id == languageId);

            return language.Name;
        }

        public async Task<List<LanguageViewModel>> GetAllInBookCategory(int categoryId)
        {
            var result = await this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.CategoryId == categoryId)
                .Select(x => new LanguageViewModel { Id = x.LanguageId, Name = x.Language.Name })
                .Distinct()
                .ToListAsync();

            return result;
        }

        public async Task<List<LanguageViewModel>> GetAllInUserBooksAsync(string userId)
        {
            var result = await this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new LanguageViewModel { Id = x.LanguageId, Name = x.Language.Name })
                .Distinct()
                .ToListAsync();

            return result;
        }
    }
}
