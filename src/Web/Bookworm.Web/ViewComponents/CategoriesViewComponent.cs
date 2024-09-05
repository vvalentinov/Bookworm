namespace Bookworm.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
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
            this.ViewData["SelectedCategoryId"] = selectedCategoryId;
            return this.View(await this.categoriesService.GetAllAsync());
        }
    }
}
