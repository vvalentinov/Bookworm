namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
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

        [HttpGet(nameof(GetUserQuotesByType))]
        public async Task<ActionResult> GetUserQuotesByType(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                string userId = this.userManager.GetUserId(this.User);
                List<QuoteViewModel> quotes = await this.retrieveUserQuotesService
                    .GetUserQuotesByTypeAsync<QuoteViewModel>(userId, quoteType);
                return new JsonResult(quotes);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet(nameof(GetAllQuotesByType))]
        public async Task<ActionResult> GetAllQuotesByType(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                string userId = this.userManager.GetUserId(this.User);
                List<QuoteViewModel> quotes = await this.retrieveQuotesService
                    .GetAllQuotesByTypeAsync(quoteType, userId);
                return new JsonResult(quotes);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet(nameof(GetLikedQuotes))]
        public async Task<IActionResult> GetLikedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            List<QuoteViewModel> quotes = await this.retrieveQuotesService
                    .GetLikedQuotesAsync(userId);
            return new JsonResult(quotes);
        }

        [HttpGet(nameof(SearchUserQuotesByContent))]
        public async Task<ActionResult> SearchUserQuotesByContent(string content, string type)
        {
            string userId = this.userManager.GetUserId(this.User);
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                List<QuoteViewModel> quotesByType = await this.searchQuoteService
                    .SearchUserQuotesByContentAndTypeAsync<QuoteViewModel>(content, userId, quoteType);
                return new JsonResult(quotesByType);
            }
            else
            {
                List<QuoteViewModel> quotes = await this.searchQuoteService
                    .SearchUserQuotesByContentAsync<QuoteViewModel>(content, userId);
                return new JsonResult(quotes);
            }
        }

        [HttpGet(nameof(GetUserApprovedQuotes))]
        public async Task<ActionResult> GetUserApprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            List<QuoteViewModel> quotesByType = await this.retrieveUserQuotesService
                    .GetUserApprovedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [HttpGet(nameof(GetUserUnapprovedQuotes))]
        public async Task<ActionResult> GetUserUnapprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            List<QuoteViewModel> quotesByType = await this.retrieveUserQuotesService
                    .GetUserUnapprovedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [HttpGet(nameof(GetUserLikedQuotes))]
        public async Task<ActionResult> GetUserLikedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            List<QuoteViewModel> quotesByType = await this.retrieveUserQuotesService
                    .GetUserLikedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [HttpGet(nameof(SearchAllQuotesByContent))]
        public async Task<ActionResult> SearchAllQuotesByContent(string content, string type)
        {
            string userId = this.userManager.GetUserId(this.User);
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                List<QuoteViewModel> quotesByType = await this.searchQuoteService
                    .SearchQuotesByContentAndTypeAsync(content, quoteType, userId);
                return new JsonResult(quotesByType);
            }
            else
            {
                List<QuoteViewModel> quotes = await this.searchQuoteService
                    .SearchQuotesByContentAsync(content, userId);
                return new JsonResult(quotes);
            }
        }

        [HttpPost(nameof(LikeQuote))]
        public async Task<int> LikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            int quoteLikes = await this.manageQuoteLikesService.LikeQuoteAsync(quoteId, userId);
            return quoteLikes;
        }

        [HttpDelete(nameof(UnlikeQuote))]
        public async Task<int> UnlikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            int quoteLikes = await this.manageQuoteLikesService.UnlikeQuoteAsync(quoteId, userId);
            return quoteLikes;
        }
    }
}
