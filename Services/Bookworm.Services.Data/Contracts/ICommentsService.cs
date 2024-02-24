namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Comments;

    public interface ICommentsService
    {
        Task CreateAsync(string userId, string content, int bookId);

        string GetCommentUserId(int commentId);

        Task DeleteAsync(int commentId, string userId);

        Task EditAsync(int commentId, string content, string userId);

        Task<SortedCommentsResponseModel> GetSortedCommentsAsync(
            ApplicationUser user,
            SortCommentsCriteria criteria);
    }
}
