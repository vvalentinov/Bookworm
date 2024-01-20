namespace Bookworm.Services.Data.Contracts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Http;

    public interface IBlobService
    {
        Task<string> UploadBlobAsync(IFormFile file, string pathPrefix = null);

        Task DeleteBlobAsync(string blobName);

        Task<Tuple<Stream, string, string>> DownloadBlobAsync(string bookId);

        string GetBlobAbsoluteUri(string fileName);

        Task<string> ReplaceBlobAsync(IFormFile file, string blobName, string path);
    }
}
