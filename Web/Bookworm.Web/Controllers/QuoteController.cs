namespace Bookworm.Web.Controllers
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class QuoteController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuotesService quotesService;

        public QuoteController(UserManager<ApplicationUser> userManager, IQuotesService quotesService)
        {
            this.userManager = userManager;
            this.quotesService = quotesService;
        }

        [Authorize]
        public IActionResult Add()
        {
            return this.View(new QuoteViewModel());
        }
    }
}
