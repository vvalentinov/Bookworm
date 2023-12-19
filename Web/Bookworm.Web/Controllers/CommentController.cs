﻿namespace Bookworm.Web.Controllers
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

        public CommentController(
            UserManager<ApplicationUser> userManager,
            ICommentsService commentsService)
        {
            this.userManager = userManager;
            this.commentsService = commentsService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([Bind(Prefix = "PostComment")]PostCommentInputModel model)
        {
            string userId = this.userManager.GetUserId(this.User);
            await this.commentsService.CreateAsync(userId, model.Content, model.BookId);
            return this.RedirectToAction("Details", "Book", new { id = model.BookId });
        }
    }
}
