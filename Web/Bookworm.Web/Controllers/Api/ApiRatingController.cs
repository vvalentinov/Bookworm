namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Ratings;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiRatingController : ApiBaseController
    {
        private readonly IRatingsService ratingService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiRatingController(
            IRatingsService ratingService,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.ratingService = ratingService;
        }

        [HttpPost(nameof(Post))]
        public async Task<ActionResult<RatingResponseModel>> Post(RatingInputModel model)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                await this.ratingService.SetRatingAsync(model.BookId, userId, model.Value);

                int votesCount = await this.ratingService.GetRatingsCountAsync(model.BookId);
                double avgVotes = await this.ratingService.GetAverageRatingAsync(model.BookId);
                int userVote = await this.ratingService.GetUserRatingAsync(model.BookId, userId);

                return new RatingResponseModel
                {
                    UserVote = userVote,
                    AverageVote = avgVotes,
                    VotesCount = votesCount,
                };
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { error = ex.Message });
            }
        }
    }
}
