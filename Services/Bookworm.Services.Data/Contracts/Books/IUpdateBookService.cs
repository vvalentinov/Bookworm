namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Authors;

    public interface IUpdateBookService
    {
        Task DeleteBookAsync(string bookId);

        Task ApproveBookAsync(string bookId);

        Task UnapproveBookAsync(string bookId);

        Task UndeleteBookAsync(string bookId);

        Task EditBookAsync(
            BookDto editBookDto,
            IEnumerable<UploadAuthorViewModel> authors,
            string userId);
    }
}
