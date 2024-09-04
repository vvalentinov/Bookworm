namespace Bookworm.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using static Bookworm.Web.StaticKeys.CacheKeys;

    public class CategoryController : BaseController
    {
        private readonly IMemoryCache memoryCache;
        private readonly ICategoriesService categoriesService;

        public CategoryController(
            IMemoryCache memoryCache,
            ICategoriesService categoriesService)
        {
            this.memoryCache = memoryCache;
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            bool categoriesAreCached = this.memoryCache.TryGetValue(
                Languages,
                out IEnumerable<CategoryViewModel> categories);

            if (!categoriesAreCached)
            {
                var result = await this.categoriesService.GetAllAsync<CategoryViewModel>();

                categories = result.Data;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                this.memoryCache.Set(
                    Languages,
                    categories,
                    cacheEntryOptions);
            }

            return this.View(categories);
        }
    }
}
