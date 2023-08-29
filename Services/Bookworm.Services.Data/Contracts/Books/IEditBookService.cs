namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEditBookService
    {
        Task EditBookAsync(
            string bookId,
            string title,
            string description,
            int categoryId,
            int languageId,
            int pagesCount,
            int publishedYear,
            string publisherName,
            IEnumerable<string> authors);
    }
}
