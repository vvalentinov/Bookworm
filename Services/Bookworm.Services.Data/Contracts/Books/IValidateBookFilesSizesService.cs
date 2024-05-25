namespace Bookworm.Services.Data.Contracts.Books
{
    using Microsoft.AspNetCore.Http;

    public interface IValidateBookFilesSizesService
    {
        void ValidateUploadedBookFileSizes(
            bool isForEdit,
            IFormFile bookFile,
            IFormFile imageFile);
    }
}
