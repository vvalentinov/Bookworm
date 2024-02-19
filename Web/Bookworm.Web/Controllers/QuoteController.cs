namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.QuoteInputModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Quotes.QuotesActionsNamesConstants;
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
        public IActionResult Upload()
        {
            return this.View(new UploadQuoteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UploadGeneralQuote(GeneralQuoteInputModel generalQuoteInputModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            try
            {
                string userId = this.userManager.GetUserId(this.User);

                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(generalQuoteInputModel);
                quoteDto.Type = QuoteType.GeneralQuote;

                await this.uploadQuoteService.UploadQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.View(nameof(this.Upload));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadMovieQuote(MovieQuoteInputModel movieQuoteInputModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            try
            {
                string userId = this.userManager.GetUserId(this.User);

                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(movieQuoteInputModel);
                quoteDto.Type = QuoteType.MovieQuote;

                await this.uploadQuoteService.UploadQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.View(nameof(this.Upload));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadBookQuote(BookQuoteInputModel bookQuoteInputModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Upload));
            }

            try
            {
                string userId = this.userManager.GetUserId(this.User);

                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(bookQuoteInputModel);
                quoteDto.Type = QuoteType.BookQuote;

                await this.uploadQuoteService.UploadQuoteAsync(quoteDto, userId);

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
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);

                var (model, actionName) = await this.retrieveQuotesService.GetQuoteForEditAsync(id, userId);

                this.ViewData["ActionName"] = actionName;

                return this.View(model);
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
        }

        [HttpPost(Name = EditGeneralQuoteAction)]
        public async Task<IActionResult> EditGeneralQuote(GeneralQuoteInputModel generalQuoteInputModel)
        {
            if (this.ModelState.IsValid == false)
            {
                this.ViewData["ActionName"] = EditGeneralQuoteAction;
                return this.View(nameof(this.Edit), generalQuoteInputModel);
            }

            try
            {
                var userId = this.userManager.GetUserId(this.User);

                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(generalQuoteInputModel);
                quoteDto.Type = QuoteType.GeneralQuote;

                await this.updateQuoteService.EditQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteEditSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Edit), new { id = generalQuoteInputModel.Id });
            }
        }

        [HttpPost(Name = EditBookQuoteAction)]
        public async Task<IActionResult> EditBookQuote(BookQuoteInputModel bookQuoteInputModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Edit), bookQuoteInputModel);
            }

            try
            {
                var userId = this.userManager.GetUserId(this.User);

                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(bookQuoteInputModel);
                quoteDto.Type = QuoteType.BookQuote;

                await this.updateQuoteService.EditQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteEditSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Edit), new { id = bookQuoteInputModel.Id });
            }
        }

        [HttpPost(Name = EditMovieQuoteAction)]
        public async Task<IActionResult> EditMovieQuote(MovieQuoteInputModel movieQuoteInputModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.Edit), movieQuoteInputModel);
            }

            try
            {
                var userId = this.userManager.GetUserId(this.User);

                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(movieQuoteInputModel);
                quoteDto.Type = QuoteType.MovieQuote;

                await this.updateQuoteService.EditQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteEditSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.Edit), new { id = movieQuoteInputModel.Id });
            }
        }

        [HttpGet]
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
        [AllowAnonymous]
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
