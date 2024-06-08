namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    public interface IManageQuoteLikesService
    {
        Task<int> LikeAsync(int quoteId, string userId);

        Task<int> UnlikeAsync(int quoteId, string userId);
    }
}
