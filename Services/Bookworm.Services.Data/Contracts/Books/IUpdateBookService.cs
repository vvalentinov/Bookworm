namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.DTOs;

    public interface IUpdateBookService
    {
        Task DeleteBookAsync(int bookId, string userId);

        Task ApproveBookAsync(int bookId);

        Task UnapproveBookAsync(int bookId);

        Task UndeleteBookAsync(int bookId);

        Task EditBookAsync(BookDto editBookDto, string userId);
    }
}
