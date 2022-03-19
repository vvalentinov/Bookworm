namespace Bookworm.Services.Data.Models
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly IConfiguration configuration;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public BlobService(
            BlobServiceClient blobServiceClient,
            IConfiguration configuration,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.blobServiceClient = blobServiceClient;
            this.configuration = configuration;
            this.bookRepository = bookRepository;
        }

        public async Task<bool> CheckIfBlobExistsAsync(string fileName)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync())
            {
                return true;
            }

            return false;
        }

        public async Task<Tuple<Stream, string, string>> DownloadBlobAsync(string bookId)
        {
            Book book = this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Id == bookId);

            book.DownloadsCount++;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            Uri uri = new Uri(book.FileUrl);

            string containerName = this.configuration.GetConnectionString("ContainerName");
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = new BlobClient(uri);

            BlobProperties blobProperties = blobClient.GetProperties();
            string contentType = blobProperties.ContentType;

            MemoryStream ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            Stream blobStream = blobClient.OpenReadAsync().Result;
            return Tuple.Create(blobStream, contentType, blobClient.Name);
        }

        public string GetBlobAbsoluteUri(string fileName)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task UploadBlobAsync(IFormFile file)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);
            byte[] fileBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            await using MemoryStream memoryStream = new MemoryStream(fileBytes);
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = file.ContentType });
        }
    }
}
