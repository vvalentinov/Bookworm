namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;
    using static Bookworm.Common.Quotes.QuotesSuccessMessagesConstants;

    public class QuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly IUploadQuoteService uploadQuoteService;
        private readonly ICheckIfQuoteExistsService checkIfQuoteExistsService;

        public QuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IUpdateQuoteService updateQuoteService,
            IUploadQuoteService uploadQuoteService,
            ICheckIfQuoteExistsService checkIfQuoteExistsService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.updateQuoteService = updateQuoteService;
            this.uploadQuoteService = uploadQuoteService;
            this.checkIfQuoteExistsService = checkIfQuoteExistsService;
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            QuoteViewModel quote = await this.retrieveQuotesService.GetQuoteByIdAsync(id);
            return this.View(quote);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(QuoteViewModel quote)
        {
            await this.updateQuoteService.EditQuoteAsync(
                quote.Id,
                quote.Content,
                quote.AuthorName,
                quote.BookTitle,
                quote.MovieTitle);

            this.TempData[MessageConstant.SuccessMessage] = QuoteEditSuccess;

            return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
        }

        [Authorize]
        public async Task<IActionResult> UserQuotes()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            UserQuotesViewModel quotes = await this.retrieveQuotesService.GetAllUserQuotesAsync(user.Id);
            return this.View(quotes);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int quoteId)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            await this.updateQuoteService.SelfQuoteDeleteAsync(quoteId, user.Id);
            this.TempData[MessageConstant.SuccessMessage] = QuoteDeleteSuccess;
            return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
        }

        public async Task<IActionResult> All()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            QuoteListingViewModel quotes = await this.retrieveQuotesService.GetAllQuotesAsync(user?.Id);
            return this.View(quotes);
        }

        [Authorize]
        public IActionResult Upload()
        {
            UploadQuoteViewModel model = new UploadQuoteViewModel();
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadGeneralQuote(UploadGeneralQuoteViewModel generalQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            if (await this.checkIfQuoteExistsService.QuoteExistsAsync(generalQuoteModel.Content))
            {
                this.TempData[MessageConstant.ErrorMessage] = QuoteExistsError;
                return this.View(nameof(this.Upload));
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.uploadQuoteService.UploadGeneralQuoteAsync(
                generalQuoteModel.Content,
                generalQuoteModel.AuthorName,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;

            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadMovieQuote(UploadMovieQuoteViewModel movieQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            if (await this.checkIfQuoteExistsService.QuoteExistsAsync(movieQuoteModel.Content))
            {
                this.TempData[MessageConstant.ErrorMessage] = QuoteExistsError;
                return this.RedirectToAction(nameof(this.Upload), "Quote");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.uploadQuoteService.UploadMovieQuoteAsync(
                movieQuoteModel.Content,
                movieQuoteModel.MovieTitle,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;

            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadBookQuote(UploadBookQuoteViewModel bookQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            if (await this.checkIfQuoteExistsService.QuoteExistsAsync(bookQuoteModel.Content))
            {
                this.TempData[MessageConstant.ErrorMessage] = QuoteExistsError;
                return this.View(nameof(this.Upload));
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.uploadQuoteService.UploadBookQuoteAsync(
                bookQuoteModel.Content,
                bookQuoteModel.BookTitle,
                bookQuoteModel.AuthorName,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;

            return this.RedirectToAction("Index", "Home");
        }
    }
}
