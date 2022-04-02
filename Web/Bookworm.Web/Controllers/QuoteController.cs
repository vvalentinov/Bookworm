namespace Bookworm.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class QuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuotesService quotesService;

        public QuoteController(UserManager<ApplicationUser> userManager, IQuotesService quotesService)
        {
            this.userManager = userManager;
            this.quotesService = quotesService;
        }

        public IActionResult All()
        {
            IEnumerable<QuoteViewModel> quotes = this.quotesService.GetAllQuotes<QuoteViewModel>();
            return this.View(quotes);
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

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added quote!";

            return this.RedirectToAction("Index", "Home");
        }
    }
}
