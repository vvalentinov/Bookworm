namespace Bookworm.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Mvc;

    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ICategoriesService categoriesService;

        public CategoriesViewComponent(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedCategoryId)
        {
            var categories = await this.categoriesService.GetAllAsync<CategoryViewModel>();
            this.ViewData["SelectedCategoryId"] = selectedCategoryId;
            return this.View(categories);
        }
    }
}
