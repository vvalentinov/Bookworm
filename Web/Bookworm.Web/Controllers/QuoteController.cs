namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(QuoteViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View();
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.quotesService.AddQuoteAsync(
                model.Content,
                model.AuthorName,
                model.BookTitle,
                model.MovieTitle,
                user.Id);

            return this.RedirectToAction("Index", "Home");
        }
    }
}
