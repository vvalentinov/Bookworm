namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.GlobalConstants;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<Vote> voteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CommentsService(
            IRepository<Comment> commentRepository,
            IRepository<Vote> voteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.commentRepository = commentRepository;
            this.voteRepository = voteRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task CreateAsync(
            string userId,
            string content,
            int bookId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Comment's content must not be empty!");
            }

            Comment comment = new Comment()
            {
                UserId = userId,
                Content = content,
                BookId = bookId,
            };

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task EditAsync(int commentId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Comment's content must not be empty!");
            }

            Comment comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment with given id not found!");
            }

            ApplicationUser user = await this.userRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("No user with given id found!");
            }

            bool isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);

            if (!isAdmin && comment.UserId != userId)
            {
                throw new InvalidOperationException("You have to be either the comment's author or an administrator to edit it!");
            }

            comment.Content = content;
            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            Comment comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment with given id not found!");
            }

            ApplicationUser user = await this.userRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            bool isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);

            if (!isAdmin && comment.UserId != userId)
            {
                throw new InvalidOperationException("You have to be either the comment's author or an administrator to delete!");
            }

            List<Vote> votes = await this.voteRepository.All().Where(v => v.CommentId == commentId).ToListAsync();
            this.voteRepository.RemoveRange(votes);
            await this.voteRepository.SaveChangesAsync();

            this.commentRepository.Delete(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public string GetCommentUserId(int commentId)
        {
            var comment = this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Id == commentId);

            return comment.UserId;
        }

        public async Task<SortedCommentsResponseModel> GetSortedCommentsAsync(
            ApplicationUser user,
            SortCommentsCriteria criteria)
        {
            List<CommentViewModel> comments = new List<CommentViewModel>();

            switch (criteria)
            {
                case SortCommentsCriteria.CreatedOnAsc:
                    comments = await this.commentRepository
                        .AllAsNoTracking()
                        .OrderBy(c => c.CreatedOn)
                        .To<CommentViewModel>()
                        .ToListAsync();
                    break;
                case SortCommentsCriteria.CreatedOnDesc:
                    comments = await this.commentRepository
                        .AllAsNoTracking()
                        .OrderByDescending(c => c.CreatedOn)
                        .To<CommentViewModel>()
                        .ToListAsync();
                    break;
                case SortCommentsCriteria.NetWorthDesc:
                    comments = await this.commentRepository
                        .AllAsNoTracking()
                        .OrderByDescending(c => c.NetWorth)
                        .To<CommentViewModel>()
                        .ToListAsync();
                    break;
            }

            if (user != null)
            {
                foreach (CommentViewModel comment in comments)
                {
                    Vote vote = await this.voteRepository
                            .AllAsNoTracking()
                            .FirstOrDefaultAsync(v =>
                                v.UserId == user.Id && v.CommentId == comment.Id);

                    comment.UserVoteValue = vote == null ? 0 : (int)vote.Value;

                    comment.IsCommentOwner = comment.UserId == user.Id;
                }
            }

            bool isUserSignedIn = user != null;
            bool isUserAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);

            SortedCommentsResponseModel model = new SortedCommentsResponseModel()
            {
                Comments = comments,
                IsUserSignedIn = isUserSignedIn,
                IsUserAdmin = isUserAdmin,
            };

            return model;
        }
    }
}
