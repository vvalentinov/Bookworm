namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface ICommentsService
    {
        Task CreateAsync(string userId, string content, string bookId);

        string GetCommentUserId(int commentId);

        Task DeleteAsync(int commentId, string userId);

        Task EditAsync(int commentId, string content, string userId);
    }
}
