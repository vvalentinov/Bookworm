namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Comments;

    public interface ICommentsService
    {
        Task CreateAsync(
            string userId,
            string content,
            int bookId);

        Task DeleteAsync(
            int commentId,
            string userId,
            bool isAdmin);

        Task EditAsync(
            int commentId,
            string content,
            string userId,
            bool isAdmin);

        Task<SortedCommentsResponseModel> GetSortedCommentsAsync(
            int bookId,
            string userId,
            string criteria,
            bool isAdmin);
    }
}
