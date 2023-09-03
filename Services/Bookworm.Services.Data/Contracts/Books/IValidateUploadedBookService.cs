namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IValidateUploadedBookService
    {
        Task ValidateUploadedBookAsync(
            IFormFile bookFile,
            IFormFile imageFile,
            IEnumerable<string> authors,
            int categoryId,
            int languageId);
    }
}
