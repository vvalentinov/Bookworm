namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class QuoteController : BaseController
    {
        private readonly IQuotesService quotesService;

        public QuoteController(IQuotesService quotesService)
        {
            this.quotesService = quotesService;
        }

        public IActionResult AllQuotes()
        {
            var quotes = this.quotesService.GetAllUnapprovedQuotes<QuoteViewModel>();
            return this.View(quotes);
        }

        public async Task<IActionResult> ApproveQuote(int id, string userId)
        {
            await this.quotesService.ApproveQuote(id, userId);
            return this.RedirectToAction(nameof(this.AllQuotes), "Quote");
        }
    }
}
