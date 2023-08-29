namespace Bookworm.Web.ViewComponents
{
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Mvc;

    public class LanguagesViewComponent : ViewComponent
    {
        private readonly ILanguagesService languagesService;

        public LanguagesViewComponent(ILanguagesService languagesService)
        {
            this.languagesService = languagesService;
        }

        public IViewComponentResult Invoke()
        {
            var languages = this.languagesService.GetAllLanguages();
            return this.View(languages);
        }
    }
}
