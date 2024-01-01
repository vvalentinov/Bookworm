namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Languages;

    public class LanguagesService : ILanguagesService
    {
        private readonly IRepository<Language> languagesRepository;

        public LanguagesService(IRepository<Language> langugesRepository)
        {
            this.languagesRepository = langugesRepository;
        }

        public IEnumerable<LanguageViewModel> GetAllLanguages()
        {
            return this.languagesRepository
                .AllAsNoTracking()
                .Select(x => new LanguageViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
        }

        public string GetLanguageName(int languageId)
        {
            return this.languagesRepository
                .AllAsNoTracking()
                .First(language => language.Id == languageId)
                .Name;
        }
    }
}
