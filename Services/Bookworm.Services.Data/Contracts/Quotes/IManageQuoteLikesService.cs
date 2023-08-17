namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    public interface IManageQuoteLikesService
    {
        Task<int> LikeQuoteAsync(int quoteId, string userId);

        Task<int> DislikeQuoteAsync(int quoteId, string userId);
    }
}
