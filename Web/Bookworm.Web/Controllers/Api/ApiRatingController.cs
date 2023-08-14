namespace Bookworm.Web.Controllers.Api
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ApiRatingController : BaseController
    {
        private readonly IRatingsService votesService;

        public ApiRatingController(IRatingsService votesService)
        {
            this.votesService = votesService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RatingResponseModel>> Post(RatingInputModel model)
        {
            string userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await this.votesService.SetVoteAsync(model.BookId, userId, model.Value);

            double avgVotes = this.votesService.GetAverageVotes(model.BookId);
            int? userVote = this.votesService.GetUserVote(model.BookId, userId);
            int votesCount = this.votesService.GetVotesCount(model.BookId);

            return new RatingResponseModel()
            {
                AverageVote = avgVotes,
                UserVote = userVote,
                VotesCount = votesCount,
            };
        }
    }
}
