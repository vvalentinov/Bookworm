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
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CommentErrorMessagesConstants;
    using static Bookworm.Common.Enums.SortCommentsCriteria;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Vote> voteRepo;
        private readonly IDeletableEntityRepository<Book> bookRepo;
        private readonly IDeletableEntityRepository<Comment> commentRepo;

        public CommentsService(
            IRepository<Vote> voteRepository,
            IDeletableEntityRepository<Comment> commentRepository,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepo = bookRepository;
            this.voteRepo = voteRepository;
            this.commentRepo = commentRepository;
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

            await this.commentRepo.AddAsync(comment);
            await this.commentRepo.SaveChangesAsync();

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

            var comment = await this.commentRepo
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

            this.commentRepo.Update(comment);
            await this.commentRepo.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteAsync(
            int commentId,
            string userId,
            bool isAdmin)
        {
            var comment = await this.commentRepo
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

            this.voteRepo.RemoveRange([.. comment.Votes]);

            this.commentRepo.Delete(comment);
            await this.commentRepo.SaveChangesAsync();

            return OperationResult.Ok("Successfully deleted comment!");
        }

        public async Task<OperationResult<SortedCommentsResponseModel>> GetSortedCommentsAsync(
            int bookId,
            string userId,
            bool isAdmin,
            string criteria = "CreatedOnDesc",
            int page = 1)
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

            var query = this.commentRepo
                .AllAsNoTracking()
                .Include(c => c.Votes)
                .Where(x => x.BookId == bookId);

            var recordsCount = await query.CountAsync();

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

            var comments = await query
                .Skip((page - 1) * 5)
                .Take(5)
                .Select(comment => new CommentViewModel
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedOn = comment.CreatedOn,
                    NetWorth = comment.NetWorth,
                    UserId = comment.UserId,
                    UserUserName = comment.User.UserName,
                    Votes = comment.Votes,
                    IsCommentOwner = userId != null && comment.UserId == userId,
                    UserVoteValue = userId == null ? 0 : comment.Votes
                        .Where(v => v.UserId == userId && v.CommentId == comment.Id)
                        .Select(v => (int?)v.Value)
                        .FirstOrDefault() ?? 0,
                })
                .ToListAsync();

            var model = new SortedCommentsResponseModel
            {
                Comments = comments,
                IsUserAdmin = isAdmin,
                IsUserSignedIn = userId != null,
                RecordsCount = recordsCount,
                ItemsPerPage = 5,
                PageNumber = page,
            };

            return OperationResult.Ok(model);
        }

        private async Task<bool> CheckIfBookIdIsValidAsync(int bookId)
            => await this.bookRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.IsApproved && x.Id == bookId) != null;
    }
}
