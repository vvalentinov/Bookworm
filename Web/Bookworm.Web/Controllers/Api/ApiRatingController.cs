namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ApiRatingController : ControllerBase
    {
        private readonly IRatingsService ratingService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiRatingController(
            IRatingsService ratingService,
            UserManager<ApplicationUser> userManager)
        {
            this.ratingService = ratingService;
            this.userManager = userManager;
        }

        [HttpPost(nameof(Post))]
        [Authorize]
        public async Task<ActionResult<RatingResponseModel>> Post(RatingInputModel model)
        {
            string userId = this.userManager.GetUserId(this.User);
            await this.ratingService.SetRatingAsync(model.BookId, userId, model.Value);

            double avgVotes = await this.ratingService.GetAverageRatingAsync(model.BookId);
            int userVote = await this.ratingService.GetUserRatingAsync(model.BookId, userId);
            int votesCount = await this.ratingService.GetRatingsCountAsync(model.BookId);

            return new RatingResponseModel()
            {
                AverageVote = avgVotes,
                UserVote = userVote,
                VotesCount = votesCount,
            };
        }
    }
}
