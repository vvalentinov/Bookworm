namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task SetVoteAsync(string bookId, string userId, byte value);

        Task<double> GetAverageVotesAsync(string bookId);

        int? GetUserVote(string bookId, string userId);

        int GetVotesCount(string bookId);
    }
}
