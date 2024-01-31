namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    public interface IManageQuoteLikesService
    {
        Task<int> LikeQuoteAsync(int quoteId, string userId);

        Task<int> UnlikeQuoteAsync(int quoteId, string userId);
    }
}
