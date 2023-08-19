﻿namespace Bookworm.Web.Areas.Admin.Controllers
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

        public IActionResult UnapprovedQuotes()
        {
            QuoteListingViewModel unapprovedQuotes = this.retrieveQuotesService.GetAllUnapprovedQuotes();
            return this.View(unapprovedQuotes);
        }

        public IActionResult DeletedQuotes()
        {
            QuoteListingViewModel deletedQuotes = this.retrieveQuotesService.GetAllDeletedQuotes();
            return this.View(deletedQuotes);
        }

        public async Task<IActionResult> ApproveQuote(int quoteId, string userId)
        {
            await this.updateQuoteService.ApproveQuoteAsync(quoteId, userId);
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
    }
}
