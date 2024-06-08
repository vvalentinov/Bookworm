namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IVoteService
    {
        Task<int> VoteAsync(
            int commentId,
            string userId,
            bool isUpVote);
    }
}
