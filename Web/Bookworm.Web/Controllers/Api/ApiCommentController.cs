namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ApiCommentController : ControllerBase
    {
        private readonly ICommentsService commentsService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiCommentController(ICommentsService commentsService, UserManager<ApplicationUser> userManager)
        {
            this.commentsService = commentsService;
            this.userManager = userManager;
        }

        [HttpGet(nameof(this.GetSortedComments))]
        public async Task<SortedCommentsResponseModel> GetSortedComments(string criteria)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            if (Enum.TryParse(criteria, out SortCommentsCriteria sortCriteria))
            {
                SortedCommentsResponseModel responseModel = await this.commentsService.GetSortedCommentsAsync(user, sortCriteria);
                return responseModel;
            }
            else
            {
                SortedCommentsResponseModel responseModel = await this.commentsService.GetSortedCommentsAsync(user, SortCommentsCriteria.CreatedOnDesc);
                return responseModel;
            }
        }
    }
}
