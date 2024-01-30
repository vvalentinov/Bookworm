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

        [Authorize]
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
                List<QuoteViewModel> quotes = await this.retrieveQuotesService
                    .GetAllQuotesByTypeAsync<QuoteViewModel>(quoteType);
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
            List<QuoteViewModel> quotes = await this.retrieveQuotesService
                    .GetLikedQuotesAsync<QuoteViewModel>();
            return new JsonResult(quotes);
        }

        [Authorize]
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

        [Authorize]
        [HttpGet(nameof(GetUserApprovedQuotes))]
        public async Task<ActionResult> GetUserApprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            List<QuoteViewModel> quotesByType = await this.retrieveUserQuotesService
                    .GetUserApprovedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [Authorize]
        [HttpGet(nameof(GetUserUnapprovedQuotes))]
        public async Task<ActionResult> GetUserUnapprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            List<QuoteViewModel> quotesByType = await this.retrieveUserQuotesService
                    .GetUserUnapprovedQuotesAsync<QuoteViewModel>(userId);
            return new JsonResult(quotesByType);
        }

        [Authorize]
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
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                List<QuoteViewModel> quotesByType = await this.searchQuoteService
                    .SearchQuotesByContentAndTypeAsync<QuoteViewModel>(content, quoteType);
                return new JsonResult(quotesByType);
            }
            else
            {
                List<QuoteViewModel> quotes = await this.searchQuoteService
                    .SearchQuotesByContentAsync<QuoteViewModel>(content);
                return new JsonResult(quotes);
            }
        }

        [Authorize]
        [HttpPost(nameof(LikeQuote))]
        public async Task<int> LikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            return await this.manageQuoteLikesService.LikeQuoteAsync(quoteId, userId);
        }

        [Authorize]
        [HttpPost(nameof(DislikeQuote))]
        public async Task<int> DislikeQuote(int quoteId)
        {
            string userId = this.userManager.GetUserId(this.User);
            return await this.manageQuoteLikesService.DislikeQuoteAsync(quoteId, userId);
        }
    }
}
