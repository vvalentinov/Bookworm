namespace Bookworm.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
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
        public IActionResult Upload() => this.View(new UploadQuoteViewModel());

        [HttpPost]
        public async Task<IActionResult> Upload(UploadQuoteViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                this.TempData[MessageConstant.ErrorMessage] = this.GetModelStateErrors();
                return this.View(model);
            }

            try
            {
                string userId = this.userManager.GetUserId(this.User);
                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(model);
                await this.uploadQuoteService.UploadQuoteAsync(quoteDto, userId);

                this.TempData[MessageConstant.SuccessMessage] = QuoteUploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                return this.View(nameof(this.Upload));
            }
        }

        [HttpGet]
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
        public async Task<IActionResult> Edit(UploadQuoteViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.ModelState.IsValid == false)
            {
                try
                {
                    return this.View(await this.retrieveQuotesService.GetQuoteForEditAsync(model.Id, userId));
                }
                catch (Exception ex)
                {
                    this.TempData[MessageConstant.ErrorMessage] = ex.Message;
                    return this.RedirectToAction(nameof(this.UserQuotes));
                }
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
                var exceptionMsg = ex.Message;
                this.TempData[MessageConstant.ErrorMessage] = exceptionMsg;
                if (exceptionMsg == QuoteWrongIdError)
                {
                    return this.RedirectToAction(nameof(this.UserQuotes));
                }
                else
                {
                    return this.View(await this.retrieveQuotesService.GetQuoteForEditAsync(model.Id, userId));
                }
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
            quotes.PaginationController = "Quote";
            quotes.PaginationAction = nameof(this.All);

            return this.View(quotes);
        }

        private string GetModelStateErrors()
            => string.Join("\n", this.ModelState
                    .Values
                    .SelectMany(v => v.Errors)
                    .Select(x => x.ErrorMessage));
    }
}
