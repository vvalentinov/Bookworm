namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels.Ratings;
    using Microsoft.AspNetCore.Mvc;

    public class ApiRatingController : ApiBaseController
    {
        private readonly IRatingsService service;

        public ApiRatingController(IRatingsService service)
        {
            this.service = service;
        }

        [HttpPost(nameof(Post))]
        public async Task<ActionResult<RatingResponseModel>> Post(RatingInputModel model)
        {
            string userId = this.User.GetId();

            var setRatingResult = await this.service.SetRatingAsync(
                model.BookId,
                userId,
                model.Value);

            if (setRatingResult.IsFailure)
            {
                return this.BadRequest(new { error = setRatingResult.ErrorMessage });
            }

            var getRatingsCountResult = await this.service
                .GetRatingsCountAsync(model.BookId);

            if (getRatingsCountResult.IsFailure)
            {
                return this.BadRequest(new { error = getRatingsCountResult.ErrorMessage });
            }

            var getAvgRatingResult = await this.service
                .GetAverageRatingAsync(model.BookId);

            if (getAvgRatingResult.IsFailure)
            {
                return this.BadRequest(new { error = getAvgRatingResult.ErrorMessage });
            }

            var getUserRatingResult = await this.service
                .GetUserRatingAsync(model.BookId, userId);

            if (getUserRatingResult.IsFailure)
            {
                return this.BadRequest(new { error = getUserRatingResult.ErrorMessage });
            }

            var responseModel = new RatingResponseModel
            {
                UserVote = getUserRatingResult.Data,
                AverageVote = getAvgRatingResult.Data,
                VotesCount = getRatingsCountResult.Data,
            };

            return this.Ok(responseModel);
        }
    }
}
