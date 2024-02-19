namespace Bookworm.Services.Data.Models
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class BlobService : IBlobService
    {
        private readonly IConfiguration configuration;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public BlobService(
            IConfiguration configuration,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.configuration = configuration;
            this.bookRepository = bookRepository;
        }

        public async Task<Tuple<Stream, string, string>> DownloadBlobAsync(string bookId)
        {
            Book book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookId);

            if (book.IsApproved)
            {
                book.DownloadsCount++;
                this.bookRepository.Update(book);
                await this.bookRepository.SaveChangesAsync();
            }

            Uri uri = new Uri(book.FileUrl);

            string containerName = this.configuration.GetConnectionString("ContainerName");
            var blobServiceClient = this.GetBlobServiceClient();
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = new BlobClient(uri);

            BlobProperties blobProperties = blobClient.GetProperties();
            string contentType = blobProperties.ContentType;

            MemoryStream ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            Stream blobStream = blobClient.OpenReadAsync().Result;
            return Tuple.Create(blobStream, contentType, blobClient.Name);
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string path)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");

            var blobServiceClient = this.GetBlobServiceClient();

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

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
            string containerName = this.configuration.GetConnectionString("ContainerName");

            var blobServiceClient = this.GetBlobServiceClient();

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var oldBlobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await oldBlobClient.UploadAsync(stream, true);
            }

            string uniqueName = GenerateUniqueName(file, path);

            await containerClient.CreateIfNotExistsAsync();

            var blobCopyClient = containerClient.GetBlobClient($"{path}{uniqueName}");

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
            string containerName = this.configuration.GetConnectionString("ContainerName");

            var blobServiceClient = this.GetBlobServiceClient();

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private static string GenerateUniqueName(IFormFile file, string path)
         => $"{path}{Path.GetFileNameWithoutExtension(file.FileName)}{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        private BlobServiceClient GetBlobServiceClient() => new (this.configuration.GetConnectionString("StorageConnection"));
    }
}
