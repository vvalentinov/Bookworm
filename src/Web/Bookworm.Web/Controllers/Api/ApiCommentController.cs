namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.Infrastructure.Filters;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Mvc;

    public class ApiCommentController : ApiBaseController
    {
        private readonly ICommentsService service;

        public ApiCommentController(ICommentsService service)
        {
            this.service = service;
        }

        [HttpGet(nameof(this.GetSortedComments))]
        public async Task<ActionResult<SortedCommentsResponseModel>> GetSortedComments(
            string criteria,
            int bookId,
            int page = 1)
        {
            if (page <= 0)
            {
                page = 1;
            }

            var userId = this.User.GetId();
            var isAdmin = this.User.IsAdmin();

            var result = await this.service.GetSortedCommentsAsync(
                bookId,
                userId,
                isAdmin,
                criteria,
                page);

            if (result.IsSuccess)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.ErrorMessage);
        }
    }
}
