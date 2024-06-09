namespace Bookworm.Web.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using static CacheKeys;

    public class LanguagesViewComponent : ViewComponent
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILanguagesService languagesService;

        public LanguagesViewComponent(
            IMemoryCache memoryCache,
            ILanguagesService languagesService)
        {
            this.memoryCache = memoryCache;
            this.languagesService = languagesService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedLanguageId)
        {
            this.ViewData["SelectedLanguageId"] = selectedLanguageId;

            bool languagesAreCached = this.memoryCache
                .TryGetValue(Languages, out IEnumerable<LanguageViewModel> languages);

            if (!languagesAreCached)
            {
                languages = await this.languagesService.GetAllAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                this.memoryCache.Set(Languages, languages, cacheEntryOptions);
            }

            return this.View(languages);
        }
    }
}
