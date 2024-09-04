namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.Infrastructure.Filters;
    using Bookworm.Web.ViewModels.DTOs;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class QuoteController : BaseController
    {
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly IUploadQuoteService uploadQuoteService;
        private readonly IRetrieveQuotesService retrieveQuotesService;

        public QuoteController(
            IUploadQuoteService uploadQuoteService,
            IUpdateQuoteService updateQuoteService,
            IRetrieveQuotesService retrieveQuotesService)
        {
            this.updateQuoteService = updateQuoteService;
            this.uploadQuoteService = uploadQuoteService;
            this.retrieveQuotesService = retrieveQuotesService;
        }

        [HttpGet]
        public IActionResult Upload() => this.View(new UploadQuoteViewModel());

        [HttpPost]
        [QuoteValidationFilter]
        public async Task<IActionResult> Upload(UploadQuoteViewModel model)
        {
            string userId = this.User.GetId();

            var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(model);

            var result = await this.uploadQuoteService.UploadQuoteAsync(
                quoteDto,
                userId);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction("Index", "Home");
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;

            return this.RedirectToAction(nameof(this.Upload));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = this.User.GetId();

            var result = await this.retrieveQuotesService.GetQuoteForEditAsync(
                id,
                userId);

            if (result.IsSuccess)
            {
                return this.View(result.Data);
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;

            return this.RedirectToAction(nameof(this.UserQuotes));
        }

        [HttpPost]
        [QuoteValidationFilter]
        public async Task<IActionResult> Edit(UploadQuoteViewModel model)
        {
            var userId = this.User.GetId();

            var quoteDto = AutoMapperConfig.MapperInstance.Map<QuoteDto>(model);

            var result = await this.updateQuoteService.EditQuoteAsync(
                quoteDto,
                userId);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction(nameof(this.UserQuotes));
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;

            return this.RedirectToAction(nameof(this.UserQuotes));
        }

        [HttpGet]
        [PageValidationFilter]
        public async Task<IActionResult> UserQuotes(int page = 1)
        {
            var userId = this.User.GetId();

            var result = await this.retrieveQuotesService.GetAllUserQuotesAsync(
                userId,
                page);

            return this.View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int quoteId)
        {
            var userId = this.User.GetId();

            var result = await this.updateQuoteService.DeleteQuoteAsync(
                quoteId,
                userId);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;
                return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;
            return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
        }

        [HttpGet]
        [AllowAnonymous]
        [PageValidationFilter]
        public async Task<IActionResult> All(int page = 1)
        {
            var userId = this.User.GetId();

            var result = await this.retrieveQuotesService.GetAllApprovedAsync(
                page,
                userId);

            return this.View(result.Data);
        }
    }
}
