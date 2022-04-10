namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class VoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IVoteService voteService;

        public VoteController(
            UserManager<ApplicationUser> userManager,
            IVoteService voteService)
        {
            this.userManager = userManager;
            this.voteService = voteService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VoteResponseModel>> Post(VoteInputModel input)
        {
            string user = this.userManager.GetUserId(this.User);

            await this.voteService.VoteAsync(input.CommentId, user, input.IsUpVote);
            var upVotes = this.voteService.GetUpVotesCount(input.CommentId);
            var downVotes = this.voteService.GetDownVotesCount(input.CommentId);
            return new VoteResponseModel { UpVotesCount = upVotes, DownVotesCount = downVotes, CommentId = input.CommentId };
        }
    }
}
