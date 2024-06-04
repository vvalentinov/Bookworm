namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;

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
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);
                return await this.commentsService.GetSortedCommentsAsync(bookId, user.Id, criteria, isAdmin);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}
