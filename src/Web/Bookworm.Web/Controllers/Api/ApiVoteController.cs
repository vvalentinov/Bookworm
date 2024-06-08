namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiVoteController : ApiBaseController
    {
        private readonly IVoteService voteService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiVoteController(
            IVoteService voteService,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.voteService = voteService;
        }

        [HttpPost(nameof(Post))]
        public async Task<IActionResult> Post(VoteInputModel input)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                int commentNetWorth = await this.voteService.VoteAsync(input.CommentId, userId, input.IsUpVote);
                return new JsonResult(commentNetWorth);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}
