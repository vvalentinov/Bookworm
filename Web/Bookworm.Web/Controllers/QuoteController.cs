namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes.Contracts;
    using Bookworm.Web.ViewModels.Quotes.Models;
    using Bookworm.Web.ViewModels.Quotes.Models.UploadQuoteViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Quotes.QuotesSuccessMessagesConstants;

    public class QuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly IUploadQuoteService uploadQuoteService;

        public QuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IUpdateQuoteService updateQuoteService,
            IUploadQuoteService uploadQuoteService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.updateQuoteService = updateQuoteService;
            this.uploadQuoteService = uploadQuoteService;
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var quote = await this.retrieveQuotesService.GetByIdAsync<QuoteViewModel>(id);
            return this.View(quote);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(IQuoteViewModel quote)
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
        public async Task<IActionResult> UserQuotes(int id = 1)
        {
            if (id <= 0)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Page cannot be less than or equal to zero!";
                return this.RedirectToAction(nameof(this.UserQuotes), new { id = 1 });
            }

            var userId = this.userManager.GetUserId(this.User);
            var quotes = await this.retrieveQuotesService.GetAllUserQuotesAsync<UserQuoteListingViewModel>(userId, id);

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

        public async Task<IActionResult> All(int id = 1)
        {
            if (id <= 0)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Page cannot be less than or equal to zero!";
                return this.RedirectToAction(nameof(this.All), new { id = 1 });
            }

            var userId = this.userManager.GetUserId(this.User);
            var quotes = await this.retrieveQuotesService.GetAllApprovedAsync<QuoteListingViewModel>(userId, id);

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
