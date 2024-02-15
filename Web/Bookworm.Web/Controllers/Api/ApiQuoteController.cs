namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Data.Models.DTOs;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ApiQuoteController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly ISearchQuoteService searchQuoteService;
        private readonly IManageQuoteLikesService manageQuoteLikesService;

        public ApiQuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            ISearchQuoteService searchQuoteService,
            IManageQuoteLikesService manageQuoteLikesService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.searchQuoteService = searchQuoteService;
            this.manageQuoteLikesService = manageQuoteLikesService;
        }

        [HttpGet(nameof(GetQuotes))]
        public async Task<IActionResult> GetQuotes([FromQuery]GetQuotesApiRequestModel model)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);

                var quotesApiDto = AutoMapperConfig.MapperInstance.Map<GetQuotesApiDto>(model);

                var quotesModel = await this.retrieveQuotesService.GetAllByCriteriaAsync(quotesApiDto, userId);

                return new JsonResult(quotesModel) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost(nameof(LikeQuote))]
        public async Task<int> LikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            return await this.manageQuoteLikesService.LikeAsync(quoteId, userId);
        }

        [HttpDelete(nameof(UnlikeQuote))]
        public async Task<int> UnlikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            return await this.manageQuoteLikesService.UnlikeAsync(quoteId, userId);
        }
    }
}
