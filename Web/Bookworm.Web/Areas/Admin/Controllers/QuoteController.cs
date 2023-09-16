namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
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

        public async Task<IActionResult> ApprovedQuotes()
        {
            QuoteListingViewModel approvedQuotes = await this.retrieveQuotesService.GetAllApprovedQuotesAsync();
            return this.View(approvedQuotes);
        }

        public async Task<IActionResult> UnapprovedQuotes()
        {
            QuoteListingViewModel unapprovedQuotes = await this.retrieveQuotesService.GetAllUnapprovedQuotesAsync();
            return this.View(unapprovedQuotes);
        }

        public async Task<IActionResult> DeletedQuotes()
        {
            QuoteListingViewModel deletedQuotes = await this.retrieveQuotesService.GetAllDeletedQuotesAsync();
            return this.View(deletedQuotes);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveQuote(int quoteId)
        {
            await this.updateQuoteService.ApproveQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.UnapprovedQuotes), "Quote");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int quoteId)
        {
            await this.updateQuoteService.DeleteQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.UnapprovedQuotes), "Quote");
        }

        [HttpPost]
        public async Task<IActionResult> Undelete(int quoteId)
        {
            await this.updateQuoteService.UndeleteQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.DeletedQuotes), "Quote");
        }

        [HttpPost]
        public async Task<IActionResult> Unapprove(int quoteId)
        {
            await this.updateQuoteService.UnapproveQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.ApprovedQuotes), "Quote");
        }
    }
}
