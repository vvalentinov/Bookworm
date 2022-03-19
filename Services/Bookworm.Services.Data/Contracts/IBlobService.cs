namespace Bookworm.Services.Data.Contracts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IBlobService
    {
        Task UploadBlobAsync(IFormFile file);

        Task<Tuple<Stream, string, string>> DownloadBlobAsync(string bookId);

        string GetBlobAbsoluteUri(string fileName);

        Task<bool> CheckIfBlobExistsAsync(string fileName);
    }
}
