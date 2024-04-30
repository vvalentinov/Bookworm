namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Constants;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class CommentController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICommentsService commentsService;

        public CommentController(
            UserManager<ApplicationUser> userManager,
            ICommentsService commentsService)
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
            catch (InvalidOperationException exception)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = model.BookId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int deleteCommentId, string bookId)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                await this.commentsService.DeleteAsync(deleteCommentId, userId);
                this.TempData[TempDataMessageConstant.SuccessMessage] = "Successfully deleted comment!";
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (InvalidOperationException exception)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int editCommentId, string content, string bookId)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                await this.commentsService.EditAsync(editCommentId, content, userId);
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (InvalidOperationException exception)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
        }
    }
}
