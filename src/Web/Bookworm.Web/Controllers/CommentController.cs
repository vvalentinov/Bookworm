namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class CommentController : BaseController
    {
        private readonly ICommentsService commentsService;

        public CommentController(ICommentsService commentsService)
        {
            this.commentsService = commentsService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([Bind(Prefix = "PostComment")] PostCommentInputModel model)
        {
            string userId = this.User.GetId();

            var result = await this.commentsService.CreateAsync(
                userId,
                model.Content,
                model.BookId);

            if (result.IsSuccess)
            {
                return this.RedirectToAction(
                    "Details",
                    "Book",
                    new { id = model.BookId });
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;

            return this.RedirectToAction(
                "Details",
                "Book",
                new { id = model.BookId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(
            int deleteCommentId,
            string bookId)
        {
            var userId = this.User.GetId();
            var isAdmin = this.User.IsAdmin();

            var result = await this.commentsService.DeleteAsync(
                deleteCommentId,
                userId,
                isAdmin);

            if (result.IsSuccess)
            {
                this.TempData[SuccessMessage] = result.SuccessMessage;

                return this.RedirectToAction(
                    "Details",
                    "Book",
                    new { id = bookId });
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;

            return this.RedirectToAction(
                "Details",
                "Book",
                new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int editCommentId,
            string content,
            string bookId)
        {
            var userId = this.User.GetId();
            var isAdmin = this.User.IsAdmin();

            var result = await this.commentsService.EditAsync(
                editCommentId,
                content,
                userId,
                isAdmin);

            if (result.IsSuccess)
            {
                return this.RedirectToAction(
                    "Details",
                    "Book",
                    new { id = bookId });
            }

            this.TempData[ErrorMessage] = result.ErrorMessage;

            return this.RedirectToAction(
                "Details",
                "Book",
                new { id = bookId });
        }
    }
}
