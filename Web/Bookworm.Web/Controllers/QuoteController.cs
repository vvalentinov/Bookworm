namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Quotes.QuotesSuccessMessagesConstants;

    public class QuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IRetrieveUserQuotesService retrieveUserQuotesService;
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly IUploadQuoteService uploadQuoteService;
        private readonly ICheckIfQuoteExistsService checkIfQuoteExistsService;

        public QuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IRetrieveUserQuotesService retrieveUserQuotesService,
            IUpdateQuoteService updateQuoteService,
            IUploadQuoteService uploadQuoteService,
            ICheckIfQuoteExistsService checkIfQuoteExistsService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.retrieveUserQuotesService = retrieveUserQuotesService;
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
            UserQuoteListingViewModel quotes = await this.retrieveUserQuotesService.GetAllUserQuotesAsync(user.Id);
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

            try
            {
                string userId = this.userManager.GetUserId(this.User);

                await this.uploadQuoteService.UploadGeneralQuoteAsync(
                    generalQuoteModel.Content.Trim(),
                    generalQuoteModel.AuthorName.Trim(),
                    userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.View(nameof(this.Upload));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadMovieQuote(UploadMovieQuoteViewModel movieQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            try
            {
                string userId = this.userManager.GetUserId(this.User);

                await this.uploadQuoteService.UploadMovieQuoteAsync(
                    movieQuoteModel.Content,
                    movieQuoteModel.MovieTitle,
                    userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.View(nameof(this.Upload));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadBookQuote(UploadBookQuoteViewModel bookQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            try
            {
                string userId = this.userManager.GetUserId(this.User);

                await this.uploadQuoteService.UploadBookQuoteAsync(
                    bookQuoteModel.Content,
                    bookQuoteModel.BookTitle,
                    bookQuoteModel.AuthorName,
                    userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.View(nameof(this.Upload));
            }
        }
    }
}
