namespace Bookworm.Services.Data.Models
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Bookworm.Common.Options;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public class BlobService : IBlobService
    {
        private readonly AzureBlobStorageOptions azureBlobStorageOptions;

        public BlobService(IOptions<AzureBlobStorageOptions> options)
        {
            this.azureBlobStorageOptions = options.Value;
        }

        public async Task<Tuple<Stream, string, string>> DownloadBlobAsync(string fileUrl)
        {
            var uri = new Uri(fileUrl);

            var blobClient = new BlobClient(uri);

            var blobProperties = await blobClient.GetPropertiesAsync();

            var contentType = blobProperties.Value.ContentType;

            using var ms = new MemoryStream();

            await blobClient.DownloadToAsync(ms);

            var blobStream = await blobClient.OpenReadAsync();

            return Tuple.Create(blobStream, contentType, blobClient.Name);
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string path)
        {
            var blobServiceClient = this.GetBlobServiceClient();

            var containerClient = blobServiceClient.GetBlobContainerClient(this.GetContainerName());

            string uniqueName = GenerateUniqueName(file, path);

            var blobClient = containerClient.GetBlobClient(uniqueName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return Uri.UnescapeDataString(blobClient.Uri.AbsoluteUri);
        }

        public async Task<string> ReplaceBlobAsync(IFormFile file, string blobName, string path)
        {
            var blobServiceClient = this.GetBlobServiceClient();

            var containerClient = blobServiceClient.GetBlobContainerClient(this.GetContainerName());

            var oldBlobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await oldBlobClient.UploadAsync(stream, true);
            }

            string uniqueName = GenerateUniqueName(file, path);

            await containerClient.CreateIfNotExistsAsync();

            var blobCopyClient = containerClient.GetBlobClient(uniqueName);

            if (!await blobCopyClient.ExistsAsync())
            {
                var newBlobClient = containerClient.GetBlobClient(blobName);

                if (await newBlobClient.ExistsAsync())
                {
                    await blobCopyClient.StartCopyFromUriAsync(newBlobClient.Uri);
                    await newBlobClient.DeleteIfExistsAsync();
                }
            }

            return Uri.UnescapeDataString(blobCopyClient.Uri.AbsoluteUri);
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var blobServiceClient = this.GetBlobServiceClient();

            var containerClient = blobServiceClient.GetBlobContainerClient(this.GetContainerName());

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private static string GenerateUniqueName(IFormFile file, string path)
            => $"{path}{Path.GetFileNameWithoutExtension(file.FileName)}{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        private BlobServiceClient GetBlobServiceClient() => new (this.azureBlobStorageOptions.StorageConnection);

        private string GetContainerName() => this.azureBlobStorageOptions.ContainerName;
    }
}
