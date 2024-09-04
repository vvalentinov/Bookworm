namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.CommentErrorMessagesConstants;
    using static Bookworm.Common.Enums.VoteValue;

    public class VotesService : IVoteService
    {
        private readonly IDeletableEntityRepository<Comment> commentRepository;

        public VotesService(IDeletableEntityRepository<Comment> commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public async Task<OperationResult<int>> VoteAsync(
            int commentId,
            string userId,
            bool isUpVote)
        {
            var comment = await this.commentRepository
                .All()
                .Include(x => x.Votes)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return OperationResult.Fail<int>(CommentWrongIdError);
            }

            if (comment.UserId == userId)
            {
                return OperationResult.Fail<int>(CommentVoteError);
            }

            var newVoteValue = isUpVote ? UpVote : DownVote;

            var vote = comment
                .Votes
                .FirstOrDefault(v => v.UserId == userId);

            if (vote == null)
            {
                comment.Votes.Add(new Vote
                {
                    UserId = userId,
                    Value = newVoteValue,
                    CommentId = commentId,
                });
            }
            else if (vote.Value != newVoteValue)
            {
                vote.Value = newVoteValue;
            }

            int upVotesCount = comment
                .Votes
                .Where(v => v.Value == UpVote)
                .Count();

            int downVotesCount = comment
                .Votes
                .Where(v => v.Value == DownVote)
                .Count();

            int netWorth = upVotesCount - downVotesCount;

            comment.NetWorth = netWorth;
            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();

            return OperationResult.Ok(netWorth);
        }
    }
}
