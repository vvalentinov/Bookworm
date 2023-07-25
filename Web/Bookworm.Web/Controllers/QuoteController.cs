namespace Bookworm.Web.Controllers
{
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

        [Authorize]
        public IActionResult Edit(int id)
        {
            QuoteViewModel quote = this.quotesService.GetQuoteById(id);
            return this.View(quote);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(QuoteViewModel quote)
        {
            await this.quotesService.EditQuoteAsync(
                quote.Id,
                quote.Content,
                quote.AuthorName,
                quote.BookTitle,
                quote.MovieTitle);

            this.TempData[MessageConstant.SuccessMessage] = "Successfully edited quote";

            return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
        }

        [Authorize]
        public async Task<IActionResult> UserQuotes()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            var quotes = this.quotesService.GetUserQuotes(user.Id);

            return this.View(quotes);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.quotesService.DeleteQuoteAsync(id);
            this.TempData[MessageConstant.SuccessMessage] = "Successfully deleted quote!";
            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult All(int page = 1)
        {
            QuoteListingViewModel quotes = this.quotesService.GetAllQuotes(page, 6);
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
