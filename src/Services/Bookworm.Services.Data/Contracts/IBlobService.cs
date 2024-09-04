namespace Bookworm.Services.Data.Contracts
{
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IBlobService
    {
        Task<string> UploadBlobAsync(
            IFormFile file,
            string? pathPrefix = null);

        Task DeleteBlobAsync(string blobName);

        Task<(Stream stream, string contentType, string downloadName)> DownloadBlobAsync(string url);

        Task<string> ReplaceBlobAsync(
            IFormFile file,
            string blobName,
            string path);
    }
}
