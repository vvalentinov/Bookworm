namespace Bookworm.Web.Controllers
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;

    public class CategoryController : BaseController
    {
        private readonly IDistributedCache cache;
        private readonly ICategoriesService categoriesService;

        public CategoryController(
            IDistributedCache cache,
            ICategoriesService categoriesService)
        {
            this.cache = cache;
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            string cachedTypes = await this.cache.GetStringAsync("categories");

            if (cachedTypes == null)
            {
                var result = await this.categoriesService.GetAllAsync<CategoryViewModel>();

                cachedTypes = JsonSerializer.Serialize(result);

                await this.cache.SetStringAsync("categories", cachedTypes);
            }

            var cahcheResult = JsonSerializer.Deserialize<List<CategoryViewModel>>(cachedTypes);

            return this.View(cahcheResult);
        }
    }
}
