namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    public interface IValidateUploadedBookService
    {
        void ValidateUploadedBook(
            IFormFile bookFile,
            IFormFile imageFile,
            IEnumerable<string> authors);
    }
}
