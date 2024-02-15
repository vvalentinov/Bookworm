namespace Bookworm.Services.Data.Contracts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IBlobService
    {
        Task<string> UploadBlobAsync(IFormFile file, string pathPrefix = null);

        Task DeleteBlobAsync(string blobName);

        Task<Tuple<Stream, string, string>> DownloadBlobAsync(string bookId);

        Task<string> ReplaceBlobAsync(IFormFile file, string blobName, string path);
    }
}
