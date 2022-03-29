namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IVoteService
    {
        Task VoteAsync(int commentId, string userId, bool isUpVote);

        int GetUpVotesCount(int commentId);

        int GetDownVotesCount(int commentId);
    }
}
