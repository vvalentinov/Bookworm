namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class CommentController : BaseController
    {
        private readonly ICommentsService commentsService;
        private readonly UserManager<ApplicationUser> userManager;

        public CommentController(
            ICommentsService commentsService,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.commentsService = commentsService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([Bind(Prefix = "PostComment")] PostCommentInputModel model)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                await this.commentsService.CreateAsync(userId, model.Content, model.BookId);
                return this.RedirectToAction("Details", "Book", new { id = model.BookId });
            }
            catch (Exception exception)
            {
                this.TempData[ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = model.BookId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int deleteCommentId, string bookId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);
                await this.commentsService.DeleteAsync(deleteCommentId, user.Id, isAdmin);
                this.TempData[SuccessMessage] = "Successfully deleted comment!";
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (Exception exception)
            {
                this.TempData[ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int editCommentId, string content, string bookId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);
                await this.commentsService.EditAsync(editCommentId, content, user.Id, isAdmin);
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (Exception exception)
            {
                this.TempData[ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
        }
    }
}
