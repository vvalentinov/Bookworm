namespace Bookworm.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;

    public class CategoryController : BaseController
    {
        private readonly ICategoriesService categoriesService;
        private readonly IDistributedCache cache;

        public CategoryController(ICategoriesService categoriesService, IDistributedCache cache)
        {
            this.categoriesService = categoriesService;
            this.cache = cache;
        }

        public async Task<IActionResult> All()
        {
            string cachedTypes = await this.cache.GetStringAsync("categories");
            if (cachedTypes == null)
            {
                var result = await this.categoriesService.GetAllAsync<CategoryViewModel>();
                cachedTypes = JsonSerializer.Serialize(result);

                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromHours(1),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3),
                };

                await this.cache.SetStringAsync("categories", cachedTypes);
            }

            List<CategoryViewModel> cahcheResult = JsonSerializer.Deserialize<List<CategoryViewModel>>(cachedTypes);

            return this.View(cahcheResult);
        }
    }
}
