namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common;

    public interface IRatingsService
    {
        Task<OperationResult> SetRatingAsync(
            int bookId,
            string userId,
            byte value);

        Task<OperationResult<double>> GetAverageRatingAsync(int bookId);

        Task<OperationResult<int>> GetUserRatingAsync(
            int bookId,
            string userId);

        Task<OperationResult<int>> GetRatingsCountAsync(int bookId);
    }
}
