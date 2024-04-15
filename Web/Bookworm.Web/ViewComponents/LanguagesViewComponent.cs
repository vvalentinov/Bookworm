namespace Bookworm.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Mvc;

    public class LanguagesViewComponent : ViewComponent
    {
        private readonly ILanguagesService languagesService;

        public LanguagesViewComponent(ILanguagesService languagesService)
        {
            this.languagesService = languagesService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedLanguageId)
        {
            this.ViewData["SelectedLanguageId"] = selectedLanguageId;
            return this.View(await this.languagesService.GetAllAsync());
        }
    }
}
