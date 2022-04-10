namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Mvc;

    public class CommentController : BaseController
    {
        private readonly ICommentsService commentsService;

        public CommentController(ICommentsService commentsService)
        {
            this.commentsService = commentsService;
        }

        public async Task<IActionResult> Delete(int commentId)
        {
            await this.commentsService.DeleteAsync(commentId);
            this.TempData[MessageConstant.SuccessMessage] = "Successfully deleted comment!";
            return this.RedirectToAction("Index", "Home", new { area = " " });
        }
    }
}
