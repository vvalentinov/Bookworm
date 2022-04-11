namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

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

        public CommentController(UserManager<ApplicationUser> userManager, ICommentsService commentsService)
        {
            this.userManager = userManager;
            this.commentsService = commentsService;
        }

        [Authorize]
        public IActionResult Create(string bookId, string bookTitle)
        {
            CreateCommentInputModel model = new CreateCommentInputModel()
            {
                BookId = bookId,
                BookTitle = bookTitle,
            };
            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateCommentInputModel model)
        {
            string userId = this.userManager.GetUserId(this.User);
            await this.commentsService.Create(userId, model.Content, model.BookId);
            return this.Redirect("/");
        }
    }
}
