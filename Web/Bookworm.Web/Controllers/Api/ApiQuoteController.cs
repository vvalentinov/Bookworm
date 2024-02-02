namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApiQuoteController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IRetrieveUserQuotesService retrieveUserQuotesService;
        private readonly ISearchQuoteService searchQuoteService;
        private readonly IManageQuoteLikesService manageQuoteLikesService;

        public ApiQuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IRetrieveUserQuotesService retrieveUserQuotesService,
            ISearchQuoteService searchQuoteService,
            IManageQuoteLikesService manageQuoteLikesService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.retrieveUserQuotesService = retrieveUserQuotesService;
            this.searchQuoteService = searchQuoteService;
            this.manageQuoteLikesService = manageQuoteLikesService;
        }

        [HttpGet(nameof(GetQuotes))]
        public async Task<IActionResult> GetQuotes(
            string type,
            string sortCriteria,
            string content)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);

                var quotes = await this.retrieveQuotesService.GetAllQuotesByTypeAsync(
                    sortCriteria,
                    userId,
                    type,
                    content);

                return new JsonResult(quotes);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet(nameof(GetLikedQuotes))]
        public async Task<IActionResult> GetLikedQuotes(string sortCriteria, string content)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);

                var quotes = await this.retrieveQuotesService.GetLikedQuotesAsync(
                    userId,
                    sortCriteria,
                    content);

                return new JsonResult(quotes);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet(nameof(SearchAllQuotesByContent))]
        public async Task<JsonResult> SearchAllQuotesByContent(string content, string type)
        {
            string userId = this.userManager.GetUserId(this.User);
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                var quotesByType = await this.searchQuoteService.SearchQuotesByContentAndTypeAsync(
                    content,
                    quoteType,
                    userId);
                return new JsonResult(quotesByType);
            }
            else
            {
                var quotes = await this.searchQuoteService.SearchQuotesByContentAsync(content, userId);
                return new JsonResult(quotes);
            }
        }

        [HttpGet(nameof(SearchLikedQuotesByContent))]
        public async Task<JsonResult> SearchLikedQuotesByContent(string content)
        {
            string userId = this.userManager.GetUserId(this.User);
            return new JsonResult(await this.searchQuoteService.SearchLikedQuotesByContentAsync(content, userId));
        }

        [HttpPost(nameof(LikeQuote))]
        public async Task<int> LikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            return await this.manageQuoteLikesService.LikeQuoteAsync(quoteId, userId);
        }

        [HttpDelete(nameof(UnlikeQuote))]
        public async Task<int> UnlikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            return await this.manageQuoteLikesService.UnlikeQuoteAsync(quoteId, userId);
        }
    }
}
