namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
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

        [Authorize]
        [HttpGet(nameof(GetUserQuotesByType))]
        public ActionResult GetUserQuotesByType(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                string userId = this.userManager.GetUserId(this.User);
                List<QuoteViewModel> quotes = this.retrieveQuotesService.GetQuotesByType(userId, quoteType);
                return new JsonResult(quotes);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet(nameof(GetAllQuotesByType))]
        public ActionResult GetAllQuotesByType(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                List<QuoteViewModel> quotes = this.retrieveQuotesService.GetQuotesByType(null, quoteType);
                return new JsonResult(quotes);
            }
            else
            {
                return this.NotFound();
            }
        }

        [Authorize]
        [HttpGet(nameof(SearchUserQuotesByContent))]
        public ActionResult SearchUserQuotesByContent(string content, string type)
        {
            string userId = this.userManager.GetUserId(this.User);
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                List<QuoteViewModel> quotesByType = this.searchQuoteService.SearchQuoteByContent(content, userId, quoteType);
                return new JsonResult(quotesByType);
            }
            else
            {
                List<QuoteViewModel> quotes = this.searchQuoteService.SearchQuoteByContent(content, userId);
                return new JsonResult(quotes);
            }
        }

        [HttpGet(nameof(SearchAllQuotesByContent))]
        public ActionResult SearchAllQuotesByContent(string content, string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                List<QuoteViewModel> quotesByType = this.searchQuoteService.SearchQuoteByContent(content, null, quoteType);
                return new JsonResult(quotesByType);
            }
            else
            {
                List<QuoteViewModel> quotes = this.searchQuoteService.SearchQuoteByContent(content, null);
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
