namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IRatingsService
    {
        Task SetVoteAsync(string bookId, string userId, byte value);

        double GetAverageVotes(string bookId);

        int? GetUserVote(string bookId, string userId);

        int GetVotesCount(string bookId);
    }
}
