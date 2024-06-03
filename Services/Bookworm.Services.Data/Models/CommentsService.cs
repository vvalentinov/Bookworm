namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.CommentErrorMessagesConstants;
    using static Bookworm.Common.Enums.SortCommentsCriteria;

    public class CommentsService : ICommentsService
    {
        private readonly IUsersService usersService;
        private readonly IRepository<Vote> voteRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public CommentsService(
            IUsersService usersService,
            IRepository<Vote> voteRepository,
            IRepository<Comment> commentRepository,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.usersService = usersService;
            this.bookRepository = bookRepository;
            this.voteRepository = voteRepository;
            this.commentRepository = commentRepository;
        }

        public async Task CreateAsync(string userId, string content, int bookId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(CommentContentEmptyError);
            }

            var comment = new Comment { UserId = userId, BookId = bookId, Content = content };

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task EditAsync(int commentId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(CommentContentEmptyError);
            }

            var comment = await this.commentRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == commentId) ??
                throw new InvalidOperationException(CommentWrongIdError);

            bool isAdmin = await this.usersService.IsUserAdminAsync(userId);

            if (!isAdmin && comment.UserId != userId)
            {
                throw new InvalidOperationException(CommentEditError);
            }

            comment.Content = content;
            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment = await this.commentRepository
                .All()
                .Include(x => x.Votes)
                .FirstOrDefaultAsync(x => x.Id == commentId) ??
                throw new InvalidOperationException(CommentWrongIdError);

            bool isAdmin = await this.usersService.IsUserAdminAsync(userId);

            if (!isAdmin && comment.UserId != userId)
            {
                throw new InvalidOperationException(CommentDeleteError);
            }

            this.voteRepository.RemoveRange([.. comment.Votes]);
            this.commentRepository.Delete(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task<SortedCommentsResponseModel> GetSortedCommentsAsync(
            int bookId,
            string userId,
            string criteria)
        {
            var bookIsValid = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookId && x.IsApproved) != null;

            if (!bookIsValid)
            {
                throw new InvalidOperationException("No book found with id!");
            }

            var isCriteriaValid = Enum.TryParse(criteria, out SortCommentsCriteria parsedCriteria);

            if (!isCriteriaValid)
            {
                throw new InvalidOperationException("Invalid sort criteria!");
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
                    var userVote = comment.Votes.FirstOrDefault(v => Predicate(v, comment.Id));
                    comment.IsCommentOwner = comment.UserId == userId;
                    comment.UserVoteValue = userVote == null ? 0 : (int)userVote.Value;
                }
            }

            var model = new SortedCommentsResponseModel
            {
                Comments = comments,
                IsUserSignedIn = userId != null,
                IsUserAdmin = await this.usersService.IsUserAdminAsync(userId),
            };

            return model;
        }
    }
}
