namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Mvc;

    public class ApiVoteController : ApiBaseController
    {
        private readonly IVoteService service;

        public ApiVoteController(IVoteService service)
        {
            this.service = service;
        }

        [HttpPost(nameof(Post))]
        public async Task<IActionResult> Post(VoteInputModel input)
        {
            string userId = this.User.GetId();

            var result = await this.service.VoteAsync(
                input.CommentId,
                userId,
                input.IsUpVote);

            if (result.IsSuccess)
            {
                return new JsonResult(result.Data);
            }

            return this.BadRequest(result.ErrorMessage);
        }
    }
}
