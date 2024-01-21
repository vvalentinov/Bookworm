namespace Bookworm.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Languages;
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
            List<LanguageViewModel> languages = await this.languagesService.GetAllAsync();
            this.ViewData["SelectedLanguageId"] = selectedLanguageId;
            return this.View(languages);
        }
    }
}
