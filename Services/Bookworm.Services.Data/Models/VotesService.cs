namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts;

    public class VotesService : IVoteService
    {
        private readonly IRepository<Vote> voteRepository;

        public VotesService(IRepository<Vote> voteRepository)
        {
            this.voteRepository = voteRepository;
        }

        public async Task<int> VoteAsync(int commentId, string userId, bool isUpVote)
        {
            Vote vote = this.voteRepository
                .All()
                .FirstOrDefault(v => v.UserId == userId && v.CommentId == commentId);

            if (vote == null)
            {
                vote = new Vote()
                {
                    UserId = userId,
                    CommentId = commentId,
                    Value = isUpVote ? VoteValue.UpVote : VoteValue.DownVote,
                };

                await this.voteRepository.AddAsync(vote);
            }
            else
            {
                vote.Value = isUpVote ? VoteValue.UpVote : VoteValue.DownVote;
            }

            await this.voteRepository.SaveChangesAsync();

            int commentNetWorth = this.GetCommentNetWorth(commentId);

            return commentNetWorth;
        }

        private int GetCommentNetWorth(int commentId)
        {
            int downVotesCount = this.voteRepository
                .AllAsNoTracking()
                .Where(v => v.Value == VoteValue.DownVote && v.CommentId == commentId)
                .Count();

            int upVotesCount = this.voteRepository
                .AllAsNoTracking()
                .Where(v => v.Value == VoteValue.UpVote && v.CommentId == commentId)
                .Count();

            int netWorth = upVotesCount - downVotesCount;

            return netWorth;
        }
    }
}
