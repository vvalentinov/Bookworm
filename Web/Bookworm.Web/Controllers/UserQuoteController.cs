namespace Bookworm.Web.Controllers
{
    using System;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Enums;
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
        [HttpGet(nameof(GetQuotes))]
        public ActionResult GetQuotes(string type)
        {
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                string userId = this.userManager.GetUserId(this.User);
                var quotes = this.quotesService.GetQuotesByType(userId, quoteType);
                JsonResult jsonResult = new JsonResult(quotes);

                return jsonResult;
            }
            else
            {
                return this.NotFound();
            }
        }

        [Authorize]
        [HttpGet(nameof(SearchQuote))]
        public ActionResult SearchQuote(string content, string type)
        {
            string userId = this.userManager.GetUserId(this.User);
            if (Enum.TryParse(type, out QuoteType quoteType))
            {
                var quotesByType = this.quotesService.SearchQuote(content, userId, quoteType);
                JsonResult jsonQuotesByType = new JsonResult(quotesByType);

                return jsonQuotesByType;
            }
            else
            {
                var quotes = this.quotesService.SearchQuote(content, userId);
                JsonResult jsonResult = new JsonResult(quotes);

                return jsonResult;
            }
        }
    }
}
