namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Authors;

    public interface IUpdateBookService
    {
        Task DeleteBookAsync(string bookId);

        Task ApproveBookAsync(string bookId);

        Task UnapproveBookAsync(string bookId);

        Task UndeleteBookAsync(string bookId);

        Task EditBookAsync(
            string bookId,
            string title,
            string description,
            int categoryId,
            int languageId,
            int pagesCount,
            int publishedYear,
            string publisherName,
            IEnumerable<UploadAuthorViewModel> authors);
    }
}
