namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.Comments;

    public interface ICommentsService
    {
        Task<OperationResult> CreateAsync(
            string userId,
            string content,
            int bookId);

        Task<OperationResult> DeleteAsync(
            int commentId,
            string userId,
            bool isAdmin);

        Task<OperationResult> EditAsync(
            int commentId,
            string content,
            string userId,
            bool isAdmin);

        Task<OperationResult<SortedCommentsResponseModel>> GetSortedCommentsAsync(
            int bookId,
            string userId,
            bool isAdmin,
            string criteria = "CreatedOnDesc",
            int page = 1);
    }
}
