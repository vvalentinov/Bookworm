namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApiUserQuoteController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IRetrieveUserQuotesService retrieveUserQuotesService;
        private readonly ISearchUserQuotesService searchUserQuoteService;
        private readonly IManageQuoteLikesService manageQuoteLikesService;

        public ApiUserQuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IRetrieveUserQuotesService retrieveUserQuotesService,
            ISearchUserQuotesService searchQuoteService,
            IManageQuoteLikesService manageQuoteLikesService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.retrieveUserQuotesService = retrieveUserQuotesService;
            this.searchUserQuoteService = searchQuoteService;
            this.manageQuoteLikesService = manageQuoteLikesService;
        }

        [HttpGet(nameof(GetUserQuotesByType))]
        public async Task<IActionResult> GetUserQuotesByType(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                string userId = this.userManager.GetUserId(this.User);
                var quotes = await this.retrieveUserQuotesService.GetUserQuotesByTypeAsync<QuoteViewModel>(userId, quoteType);
                return new JsonResult(quotes);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet(nameof(GetUserApprovedQuotes))]
        public async Task<ActionResult> GetUserApprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            var quotesByType = await this.retrieveUserQuotesService
                    .GetUserApprovedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [HttpGet(nameof(GetUserUnapprovedQuotes))]
        public async Task<JsonResult> GetUserUnapprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            var quotesByType = await this.retrieveUserQuotesService.GetUserUnapprovedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [HttpGet(nameof(GetUserLikedQuotes))]
        public async Task<JsonResult> GetUserLikedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            var quotesByType = await this.retrieveUserQuotesService.GetUserLikedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }
    }
}
