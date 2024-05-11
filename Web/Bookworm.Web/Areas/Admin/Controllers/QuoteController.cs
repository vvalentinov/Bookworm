namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class QuoteController : BaseController
    {
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly UserManager<ApplicationUser> userManager;

        public QuoteController(
            IRetrieveQuotesService retrieveQuotesService,
            IUpdateQuoteService updateQuoteService,
            UserManager<ApplicationUser> userManager)
        {
            this.retrieveQuotesService = retrieveQuotesService;
            this.updateQuoteService = updateQuoteService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedQuotes()
            => this.View(await this.retrieveQuotesService.GetAllApprovedAsync());

        [HttpGet]
        public async Task<IActionResult> UnapprovedQuotes()
            => this.View(await this.retrieveQuotesService.GetAllUnapprovedAsync());

        [HttpGet]
        public async Task<IActionResult> DeletedQuotes()
            => this.View(await this.retrieveQuotesService.GetAllDeletedAsync());

        [HttpPost]
        public async Task<IActionResult> ApproveQuote(int quoteId)
        {
            await this.updateQuoteService.ApproveQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.UnapprovedQuotes));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int quoteId)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);
                await this.updateQuoteService.DeleteQuoteAsync(quoteId, userId, isCurrUserAdmin: true);

                this.TempData[SuccessMessage] = DeleteSuccess;
                return this.RedirectToAction(nameof(this.UnapprovedQuotes));
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UnapprovedQuotes));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Undelete(int quoteId)
        {
            await this.updateQuoteService.UndeleteQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.DeletedQuotes));
        }

        [HttpPost]
        public async Task<IActionResult> Unapprove(int quoteId)
        {
            await this.updateQuoteService.UnapproveQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.ApprovedQuotes));
        }
    }
}
