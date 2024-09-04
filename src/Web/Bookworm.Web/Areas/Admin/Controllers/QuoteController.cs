namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class QuoteController : BaseController
    {
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly IRetrieveQuotesService retrieveQuotesService;

        public QuoteController(
            IUpdateQuoteService updateQuoteService,
            IRetrieveQuotesService retrieveQuotesService)
        {
            this.updateQuoteService = updateQuoteService;
            this.retrieveQuotesService = retrieveQuotesService;
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedQuotes()
        {
            var result = await this.retrieveQuotesService
                .GetAllApprovedAsync();

            return this.View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> UnapprovedQuotes()
        {
            var result = await this.retrieveQuotesService
                .GetAllUnapprovedAsync();

            return this.View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> DeletedQuotes()
        {
            var result = await this.retrieveQuotesService
                .GetAllDeletedAsync();

            return this.View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveQuote(int quoteId)
        {
            var result = await this.updateQuoteService
                .ApproveQuoteAsync(quoteId);

            if (result.IsFailure)
            {
                this.TempData[ErrorMessage] = result.ErrorMessage;
            }

            return this.RedirectToAction(nameof(this.UnapprovedQuotes));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int quoteId)
        {
            var userId = this.User.GetId();

            var result = await this.updateQuoteService.DeleteQuoteAsync(
                quoteId,
                userId,
                isCurrUserAdmin: true);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction(nameof(this.UnapprovedQuotes));
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.UnapprovedQuotes));
        }

        [HttpPost]
        public async Task<IActionResult> Undelete(int quoteId)
        {
            var result = await this.updateQuoteService
                .UndeleteQuoteAsync(quoteId);

            if (result.IsFailure)
            {
                this.TempData[ErrorMessage] = result.ErrorMessage;
            }

            return this.RedirectToAction(nameof(this.DeletedQuotes));
        }

        [HttpPost]
        public async Task<IActionResult> Unapprove(int quoteId)
        {
            var result = await this.updateQuoteService
                .UnapproveQuoteAsync(quoteId);

            if (result.IsFailure)
            {
                this.TempData[ErrorMessage] = result.ErrorMessage;
            }

            return this.RedirectToAction(nameof(this.ApprovedQuotes));
        }
    }
}
