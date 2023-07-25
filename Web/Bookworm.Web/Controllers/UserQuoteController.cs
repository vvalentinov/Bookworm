namespace Bookworm.Web.Controllers
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class UserQuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuotesService quotesService;

        public UserQuoteController(UserManager<ApplicationUser> userManager, IQuotesService quotesService)
        {
            this.userManager = userManager;
            this.quotesService = quotesService;
        }

        [Authorize]
        [HttpGet("GetApprovedQuotes")]
        public ActionResult GetApprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            var quotes = this.quotesService.GetUserApprovedQuotes(userId);

            return this.Json(quotes);
        }

        [Authorize]
        [HttpGet("GetUnapprovedQuotes")]
        public ActionResult GetUnapprovedQuotes()
        {
            string userId = this.userManager.GetUserId(this.User);
            var quotes = this.quotesService.GetUserUnapprovedQuotes(userId);

            return this.Json(quotes);
        }
    }
}
