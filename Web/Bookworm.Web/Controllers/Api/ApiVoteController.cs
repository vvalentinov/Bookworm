﻿namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiVoteController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IVoteService voteService;

        public ApiVoteController(
            UserManager<ApplicationUser> userManager,
            IVoteService voteService)
        {
            this.userManager = userManager;
            this.voteService = voteService;
        }

        [Authorize]
        [HttpPost(nameof(Post))]
        public async Task<JsonResult> Post(VoteInputModel input)
        {
            string userId = this.userManager.GetUserId(this.User);

            int commentNetWorth = await this.voteService.VoteAsync(input.CommentId, userId, input.IsUpVote);

            return new JsonResult(commentNetWorth);
        }
    }
}
