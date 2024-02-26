namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.AspNetCore.Http;

    public interface IValidateUploadedBookService
    {
        Task ValidateUploadedBookAsync(
            bool isForEdit,
            int categoryId,
            int languageId,
            IFormFile bookFile,
            IFormFile imageFile,
            IEnumerable<UploadAuthorViewModel> authors);
    }
}
