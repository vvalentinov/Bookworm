namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiCommentController : ApiBaseController
    {
        private readonly ICommentsService commentsService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiCommentController(
            ICommentsService commentsService,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.commentsService = commentsService;
        }

        [HttpGet(nameof(this.GetSortedComments))]
        public async Task<ActionResult<SortedCommentsResponseModel>> GetSortedComments(string criteria, int bookId)
        {
            var userId = this.userManager.GetUserId(this.User);

            try
            {
                return await this.commentsService.GetSortedCommentsAsync(bookId, userId, criteria);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}
