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
    using Microsoft.EntityFrameworkCore;

    public class CommentsService : ICommentsService
    {
        private readonly IUsersService usersService;
        private readonly IRepository<Vote> voteRepository;
        private readonly IRepository<Comment> commentRepository;

        public CommentsService(
            IUsersService usersService,
            IRepository<Vote> voteRepository,
            IRepository<Comment> commentRepository)
        {
            this.usersService = usersService;
            this.voteRepository = voteRepository;
            this.commentRepository = commentRepository;
        }

        public async Task CreateAsync(string userId, string content, int bookId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Comment's content must not be empty!");
            }

            var comment = new Comment()
            {
                UserId = userId,
                BookId = bookId,
                Content = content,
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

            var comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId) ??
                throw new InvalidOperationException("Comment with given id not found!");

            bool isAdmin = await this.usersService.IsUserAdminAsync(userId);

            if (!isAdmin && comment.UserId != userId)
            {
                throw new InvalidOperationException("You don't have permission to edit this comment!");
            }

            comment.Content = content;
            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId) ??
                throw new InvalidOperationException("Comment with given id not found!");

            bool isAdmin = await this.usersService.IsUserAdminAsync(userId);

            if (!isAdmin && comment.UserId != userId)
            {
                throw new InvalidOperationException("You don't have permission to edit this comment!");
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

        public async Task<SortedCommentsResponseModel> GetSortedCommentsAsync(ApplicationUser user, SortCommentsCriteria criteria)
        {
            var comments = new List<CommentViewModel>();

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
                foreach (var comment in comments)
                {
                    var vote = await this.voteRepository
                            .AllAsNoTracking()
                            .FirstOrDefaultAsync(v =>
                                v.UserId == user.Id && v.CommentId == comment.Id);

                    comment.UserVoteValue = vote == null ? 0 : (int)vote.Value;

                    comment.IsCommentOwner = comment.UserId == user.Id;
                }
            }

            bool isUserSignedIn = user != null;
            bool isUserAdmin = await this.usersService.IsUserAdminAsync(user.Id);

            var model = new SortedCommentsResponseModel()
            {
                Comments = comments,
                IsUserSignedIn = isUserSignedIn,
                IsUserAdmin = isUserAdmin,
            };

            return model;
        }
    }
}
