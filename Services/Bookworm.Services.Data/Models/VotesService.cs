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

        public int GetDownVotesCount(int commentId)
        {
            return this.voteRepository
                .AllAsNoTracking()
                .Where(x => x.Value == VoteValue.DownVote && x.CommentId == commentId)
                .Count();
        }

        public int GetUpVotesCount(int commentId)
        {
            int count = this.voteRepository
                .AllAsNoTracking()
                .Where(x => x.Value == VoteValue.UpVote && x.CommentId == commentId)
                .Count();
            return count;
        }

        public async Task VoteAsync(int commentId, string userId, bool isUpVote)
        {
            Vote vote = this.voteRepository
                .All()
                .FirstOrDefault(x => x.UserId == userId && x.CommentId == commentId);

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
        }
    }
}
