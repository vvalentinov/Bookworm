namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
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

        public LanguagesService(IRepository<Language> langugesRepository)
        {
            this.languagesRepository = langugesRepository;
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
    }
}
