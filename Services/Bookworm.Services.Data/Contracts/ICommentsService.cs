namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface ICommentsService
    {
        Task Create(string userId, string content, string bookId);

        string GetCommentUserId(int commentId);
    }
}
