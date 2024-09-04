namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CommentErrorMessagesConstants;
    using static Bookworm.Common.Enums.SortCommentsCriteria;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Vote> voteRepository;
        private readonly IDeletableEntityRepository<Comment> commentRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public CommentsService(
            IRepository<Vote> voteRepository,
            IDeletableEntityRepository<Comment> commentRepository,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
            this.voteRepository = voteRepository;
            this.commentRepository = commentRepository;
        }

        public async Task<OperationResult> CreateAsync(
            string userId,
            string content,
            int bookId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return OperationResult.Fail(CommentContentEmptyError);
            }

            if (!await this.CheckIfBookIdIsValidAsync(bookId))
            {
                return OperationResult.Fail(BookWrongIdError);
            }

            var comment = new Comment
            {
                UserId = userId,
                BookId = bookId,
                Content = content,
            };

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> EditAsync(
            int commentId,
            string content,
            string userId,
            bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return OperationResult.Fail(CommentContentEmptyError);
            }

            var comment = await this.commentRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                return OperationResult.Fail(CommentWrongIdError);
            }

            if (!isAdmin && comment.UserId != userId)
            {
                return OperationResult.Fail(CommentEditError);
            }

            comment.Content = content;
            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteAsync(
            int commentId,
            string userId,
            bool isAdmin)
        {
            var comment = await this.commentRepository
                .All()
                .Include(x => x.Votes)
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                return OperationResult.Fail(CommentWrongIdError);
            }

            if (!isAdmin && comment.UserId != userId)
            {
                return OperationResult.Fail(CommentDeleteError);
            }

            this.voteRepository.RemoveRange([.. comment.Votes]);
            this.commentRepository.Delete(comment);
            await this.commentRepository.SaveChangesAsync();

            return OperationResult.Ok("Successfully deleted comment!");
        }

        public async Task<OperationResult<SortedCommentsResponseModel>> GetSortedCommentsAsync(
            int bookId,
            string userId,
            string criteria,
            bool isAdmin)
        {
            if (!await this.CheckIfBookIdIsValidAsync(bookId))
            {
                return OperationResult.Fail<SortedCommentsResponseModel>(BookWrongIdError);
            }

            var isCriteriaValid = Enum.TryParse(
                criteria,
                out SortCommentsCriteria parsedCriteria);

            if (!isCriteriaValid)
            {
                return OperationResult.Fail<SortedCommentsResponseModel>(CommentInvalidSortCriteria);
            }

            var query = this.commentRepository
                .AllAsNoTracking()
                .Include(c => c.Votes)
                .Where(x => x.BookId == bookId);

            switch (parsedCriteria)
            {
                case CreatedOnAsc:
                    query = query.OrderBy(c => c.CreatedOn);
                    break;
                case CreatedOnDesc:
                    query = query.OrderByDescending(c => c.CreatedOn);
                    break;
                case NetWorthDesc:
                    query = query.OrderByDescending(c => c.NetWorth);
                    break;
            }

            var comments = await query.To<CommentViewModel>().ToListAsync();

            if (userId != null)
            {
                bool Predicate(Vote v, int id) => v.UserId == userId && v.CommentId == id;

                foreach (var comment in comments)
                {
                    comment.IsCommentOwner = comment.UserId == userId;
                    var userVote = comment.Votes.FirstOrDefault(v => Predicate(v, comment.Id));
                    comment.UserVoteValue = userVote == null ? 0 : (int)userVote.Value;
                }
            }

            var model = new SortedCommentsResponseModel
            {
                Comments = comments,
                IsUserAdmin = isAdmin,
                IsUserSignedIn = userId != null,
            };

            return OperationResult.Ok(model);
        }

        private async Task<bool> CheckIfBookIdIsValidAsync(int bookId)
            => await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookId && x.IsApproved) != null;
    }
}
