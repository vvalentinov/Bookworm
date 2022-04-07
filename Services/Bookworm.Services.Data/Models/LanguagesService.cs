namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class LanguagesService : ILanguagesService
    {
        private readonly IRepository<Language> languagesRepository;

        public LanguagesService(IRepository<Language> langugesRepository)
        {
            this.languagesRepository = langugesRepository;
        }

        public IEnumerable<SelectListItem> GetAllLanguages()
        {
            return this.languagesRepository
                .AllAsNoTracking()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                })
                .ToList();
        }
    }
}
