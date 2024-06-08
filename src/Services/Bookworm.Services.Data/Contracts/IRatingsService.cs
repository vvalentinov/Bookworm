namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IRatingsService
    {
        Task SetRatingAsync(int bookId, string userId, byte value);

        Task<double> GetAverageRatingAsync(int bookId);

        Task<int> GetUserRatingAsync(int bookId, string userId);

        Task<int> GetRatingsCountAsync(int bookId);
    }
}
