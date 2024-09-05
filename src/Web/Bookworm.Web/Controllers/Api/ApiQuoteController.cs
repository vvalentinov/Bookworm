namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class ApiQuoteController : ApiBaseController
    {
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IManageQuoteLikesService manageQuoteLikesService;

        public ApiQuoteController(
            IRetrieveQuotesService retrieveQuotesService,
            IManageQuoteLikesService manageQuoteLikesService)
        {
            this.retrieveQuotesService = retrieveQuotesService;
            this.manageQuoteLikesService = manageQuoteLikesService;
        }

        [HttpGet(nameof(GetQuotes))]
        public async Task<IActionResult> GetQuotes([FromQuery]GetQuotesApiRequestModel model)
        {
            string userId = this.User.GetId();

            var quotesApiDto = model.MapToGetQuotesApiDto();

            var result = await this.retrieveQuotesService
                .GetAllByCriteriaAsync(userId, quotesApiDto);

            if (result.IsSuccess)
            {
                return new JsonResult(result.Data) { StatusCode = 200 };
            }

            return this.BadRequest(result.ErrorMessage);
        }

        [HttpPost(nameof(LikeQuote))]
        public async Task<ActionResult<int>> LikeQuote(int quoteId)
        {
            string userId = this.User.GetId();

            var result = await this.manageQuoteLikesService
                .LikeAsync(quoteId, userId);

            if (result.IsSuccess)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.ErrorMessage);
        }

        [HttpDelete(nameof(UnlikeQuote))]
        public async Task<ActionResult<int>> UnlikeQuote(int quoteId)
        {
            string userId = this.User.GetId();

            var result = await this.manageQuoteLikesService
                .UnlikeAsync(quoteId, userId);

            if (result.IsSuccess)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.ErrorMessage);
        }
    }
}
