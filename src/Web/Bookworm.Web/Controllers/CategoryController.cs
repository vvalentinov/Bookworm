namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CategoryController : BaseController
    {
        private readonly ICategoriesService categoriesService;

        public CategoryController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            var categories = await this.categoriesService.GetAllAsync<CategoryViewModel>();
            return this.View(categories);
        }
    }
}
