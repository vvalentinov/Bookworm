namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.AspNetCore.Http;

    public interface IUploadBookService
    {
        Task UploadBookAsync(
            string title,
            string description,
            int languageId,
            string publisher,
            int pagesCount,
            int publishedYear,
            IFormFile bookFile,
            IFormFile imageFile,
            int categoryId,
            IEnumerable<UploadAuthorViewModel> authors,
            string userId,
            string userName);
    }
}
