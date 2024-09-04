namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common;

    public interface IVoteService
    {
        Task<OperationResult<int>> VoteAsync(
            int commentId,
            string userId,
            bool isUpVote);
    }
}
