namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;

    public class LanguagesService : ILanguagesService
    {
        private readonly IRepository<Language> langugesRepository;

        public LanguagesService(IRepository<Language> langugesRepository)
        {
            this.langugesRepository = langugesRepository;
        }

        public IEnumerable<T> GetAllLanguages<T>()
        {
            return this.langugesRepository
                .AllAsNoTracking()
                .To<T>()
                .ToList();
        }
    }
}
