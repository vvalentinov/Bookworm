namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
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

        [HttpGet]
        [Authorize]
        public IActionResult Upload()
        {
            return this.View(new UploadQuoteViewModel());
        }

        [HttpPost]
        [Authorize]
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);

                var model = await this.retrieveQuotesService.GetQuoteForEditAsync(id, userId);

                return this.View(model);
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;

                return this.RedirectToAction(nameof(this.UserQuotes));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditQuoteInputModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            try
            {
                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(model);

                await this.updateQuoteService.EditQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteEditSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Edit), new { id = model.Id });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserQuotes(int id = 1)
        {
            if (id <= 0)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Page cannot be less than or equal to zero!";
                return this.RedirectToAction(nameof(this.UserQuotes), new { id = 1 });
            }

            var userId = this.userManager.GetUserId(this.User);
            var quotes = await this.retrieveQuotesService.GetAllUserQuotesAsync(userId, id);

            return this.View(quotes);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int quoteId)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);

                await this.updateQuoteService.DeleteQuoteAsync(quoteId, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteDeleteSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
            }
        }

        [HttpGet]
        public async Task<IActionResult> All(int id = 1)
        {
            if (id <= 0)
            {
                this.TempData[MessageConstant.ErrorMessage] = "Page cannot be less than or equal to zero!";
                return this.RedirectToAction(nameof(this.All), new { id = 1 });
            }

            var userId = this.userManager.GetUserId(this.User);
            var quotes = await this.retrieveQuotesService.GetAllApprovedAsync(userId, id);

            return this.View(quotes);
        }
    }
}
