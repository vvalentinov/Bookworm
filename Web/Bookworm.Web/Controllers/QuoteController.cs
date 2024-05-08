namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Infrastructure.Filters;
    using Bookworm.Web.ViewModels.DTOs;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;

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
        [QuoteValidationFilter]
        public async Task<IActionResult> Upload(UploadQuoteViewModel model)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(model);
                await this.uploadQuoteService.UploadQuoteAsync(quoteDto, userId);

                this.TempData[SuccessMessage] = UploadSuccess;
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.View(model);
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
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
        }

        [HttpPost]
        [QuoteValidationFilter]
        public async Task<IActionResult> Edit(UploadQuoteViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            try
            {
                var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(model);
                await this.updateQuoteService.EditQuoteAsync(quoteDto, userId);

                this.TempData[SuccessMessage] = EditSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }
        }

        [HttpGet]
        [PageValidationFilter]
        public async Task<IActionResult> UserQuotes(int page = 1)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quotes = await this.retrieveQuotesService.GetAllUserQuotesAsync(userId, page);

            return this.View(quotes);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int quoteId)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);
                await this.updateQuoteService.DeleteQuoteAsync(quoteId, userId);

                this.TempData[SuccessMessage] = DeleteSuccess;
                return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [PageValidationFilter]
        public async Task<IActionResult> All(int page = 1)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quotes = await this.retrieveQuotesService.GetAllApprovedAsync(page, userId);

            return this.View(quotes);
        }
    }
}
