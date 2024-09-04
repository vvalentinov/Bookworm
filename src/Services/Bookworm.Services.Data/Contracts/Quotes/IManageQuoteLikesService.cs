namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;

    public interface IManageQuoteLikesService
    {
        Task<OperationResult<int>> LikeAsync(
            int quoteId,
            string userId);

        Task<OperationResult<int>> UnlikeAsync(
            int quoteId,
            string userId);
    }
}
