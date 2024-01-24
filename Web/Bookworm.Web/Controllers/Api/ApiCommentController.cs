namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

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

        [HttpGet(nameof(this.GetSortedByDateAsc))]
        public async Task<SortedCommentsResponseModel> GetSortedByDateAsc()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            SortedCommentsResponseModel responseModel = await this.commentsService.GetSortedCommentsByDateAscAsync(user);

            return responseModel;
        }
    }
}
