namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IRatingsService
    {
        Task SetRatingAsync(string bookId, string userId, byte value);

        Task<double> GetAverageRatingAsync(string bookId);

        Task<int> GetUserRatingAsync(string bookId, string userId);

        Task<int> GetRatingsCountAsync(string bookId);
    }
}
