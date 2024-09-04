namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Mvc;

    public class ApiCommentController : ApiBaseController
    {
        private readonly ICommentsService commentsService;

        public ApiCommentController(ICommentsService commentsService)
        {
            this.commentsService = commentsService;
        }

        [HttpGet(nameof(this.GetSortedComments))]
        public async Task<ActionResult<SortedCommentsResponseModel>> GetSortedComments(
            string criteria,
            int bookId)
        {
            var userId = this.User.GetId();
            var isAdmin = this.User.IsAdmin();

            var result = await this.commentsService.GetSortedCommentsAsync(
                bookId,
                userId,
                criteria,
                isAdmin);

            if (result.IsSuccess)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.ErrorMessage);
        }
    }
}
