namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class QuoteController : BaseController
    {
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IUpdateQuoteService updateQuoteService;

        public QuoteController(
            IRetrieveQuotesService retrieveQuotesService,
            IUpdateQuoteService updateQuoteService)
        {
            this.retrieveQuotesService = retrieveQuotesService;
            this.updateQuoteService = updateQuoteService;
        }

        public IActionResult AllQuotes()
        {
            var quotes = this.retrieveQuotesService.GetAllUnapprovedQuotes<QuoteViewModel>();
            return this.View(quotes);
        }

        public async Task<IActionResult> ApproveQuote(int id, string userId)
        {
            await this.updateQuoteService.ApproveQuote(id, userId);
            return this.RedirectToAction(nameof(this.AllQuotes), "Quote");
        }

        public async Task<IActionResult> DeleteQuote(int id)
        {
            await this.updateQuoteService.DeleteQuoteAsync(id);
            return this.RedirectToAction(nameof(this.AllQuotes), "Quote");
        }
    }
}
