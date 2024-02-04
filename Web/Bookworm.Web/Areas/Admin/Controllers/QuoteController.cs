namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.GlobalConstants;

    [Authorize(Roles = AdministratorRoleName)]
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

        public async Task<IActionResult> ApprovedQuotes()
        {
            QuoteListingViewModel approvedQuotes = await this.retrieveQuotesService.GetAllApprovedAsync();
            return this.View(approvedQuotes);
        }

        public async Task<IActionResult> UnapprovedQuotes()
        {
            QuoteListingViewModel unapprovedQuotes = await this.retrieveQuotesService.GetAllUnapprovedAsync();
            return this.View(unapprovedQuotes);
        }

        public async Task<IActionResult> DeletedQuotes()
        {
            QuoteListingViewModel deletedQuotes = await this.retrieveQuotesService.GetAllDeletedAsync();
            return this.View(deletedQuotes);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
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

        [HttpPost]
        public async Task<IActionResult> Unapprove(int quoteId)
        {
            await this.updateQuoteService.UnapproveQuoteAsync(quoteId);
            return this.RedirectToAction(nameof(this.ApprovedQuotes), "Quote");
        }
    }
}
