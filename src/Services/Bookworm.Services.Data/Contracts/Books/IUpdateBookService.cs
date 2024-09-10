namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.DTOs;

    public interface IUpdateBookService
    {
        Task<OperationResult> DeleteBookAsync(
            int bookId,
            string userId,
            bool isUserAdmin = false);

        Task<OperationResult> ApproveBookAsync(int bookId);

        Task<OperationResult> UnapproveBookAsync(int bookId);

        Task<OperationResult> UndeleteBookAsync(int bookId);

        Task<OperationResult> EditBookAsync(
            BookDto bookDto,
            string userId);
    }
}
