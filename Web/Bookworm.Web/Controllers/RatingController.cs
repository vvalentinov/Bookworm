namespace Bookworm.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : BaseController
    {
        private readonly IVotesService votesService;

        public RatingController(IVotesService votesService)
        {
            this.votesService = votesService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostVoteResponseModel>> Post(PostVoteInputModel model)
        {
            string userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await this.votesService.SetVoteAsync(model.BookId, userId, model.Value);

            double avgVotes = await this.votesService.GetAverageVotesAsync(model.BookId);
            int? userVote = this.votesService.GetUserVote(model.BookId, userId);
            int votesCount = this.votesService.GetVotesCount(model.BookId);

            return new PostVoteResponseModel()
            {
                AverageVote = avgVotes,
                UserVote = userVote,
                VotesCount = votesCount,
            };
        }
    }
}
