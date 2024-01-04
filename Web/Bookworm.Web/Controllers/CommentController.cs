namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = model.BookId });
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int commentId, string bookId)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                await this.commentsService.DeleteAsync(commentId, userId);
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (InvalidOperationException exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int commentId, string content, string bookId)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                await this.commentsService.EditAsync(commentId, content, userId);
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (InvalidOperationException exception)
            {
                this.TempData[MessageConstant.ErrorMessage] = exception.Message;
                return this.RedirectToAction("Details", "Book", new { id = bookId });
            }
        }
    }
}
