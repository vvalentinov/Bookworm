namespace Bookworm.Web.Controllers
{
    using System;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class UserQuoteController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuotesService quotesService;

        public UserQuoteController(
            UserManager<ApplicationUser> userManager,
            IQuotesService quotesService)
        {
            this.userManager = userManager;
            this.quotesService = quotesService;
        }

        [Authorize]
        [HttpGet(nameof(GetUserQuotesByType))]
        public ActionResult GetUserQuotesByType(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                string userId = this.userManager.GetUserId(this.User);
                var quotes = this.quotesService.GetQuotesByType(userId, quoteType);
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
                var quotes = this.quotesService.GetQuotesByType(null, quoteType);
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
                var quotesByType = this.quotesService.SearchQuote(content, userId, quoteType);
                return new JsonResult(quotesByType);
            }
            else
            {
                var quotes = this.quotesService.SearchQuote(content, userId);
                return new JsonResult(quotes);
            }
        }

        [HttpGet(nameof(SearchAllQuotesByContent))]
        public ActionResult SearchAllQuotesByContent(string content, string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                var quotesByType = this.quotesService.SearchQuote(content, null, quoteType);
                return new JsonResult(quotesByType);
            }
            else
            {
                var quotes = this.quotesService.SearchQuote(content, null);
                return new JsonResult(quotes);
            }
        }
    }
}
