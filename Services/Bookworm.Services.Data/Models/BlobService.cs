﻿namespace Bookworm.Services.Data.Models
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage;
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
        private readonly BlobServiceClient blobServiceClient;
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

        public async Task<Tuple<Stream, string, string>> DownloadBlobAsync(string bookId)
        {
            Book book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookId);

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
            return Uri.UnescapeDataString(blobClient.Uri.AbsoluteUri);
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string path = null)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");

            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);

            string uniqueName = GenerateUniqueName(file);
            if (path != null)
            {
                uniqueName = path + uniqueName;
            }

            BlobClient blobClient = containerClient.GetBlobClient(uniqueName);

            byte[] fileBytes = GetFileBytes(file);

            await using MemoryStream memoryStream = new MemoryStream(fileBytes);

            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = file.ContentType });

            return uniqueName;
        }

        public async Task<string> ReplaceBlobAsync(IFormFile file, string blobName, string path)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");

            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            byte[] fileBytes = GetFileBytes(file);

            await using MemoryStream memoryStream = new MemoryStream(fileBytes);

            await blobClient.UploadAsync(memoryStream, overwrite: true);

            string uniqueName = GenerateUniqueName(file);

            return await this.RenameBlob(uniqueName, blobName, path);
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            string containerName = this.configuration.GetConnectionString("ContainerName");
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private static byte[] GetFileBytes(IFormFile file)
        {
            byte[] fileBytes = null;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return fileBytes;
        }

        private static string GenerateUniqueName(IFormFile file)
        {
            return $"{Path.GetFileNameWithoutExtension(file.FileName)}" +
                   $"{Guid.NewGuid()}" +
                   $"{Path.GetExtension(file.FileName)}";
        }

        private async Task<string> RenameBlob(string newFileName, string oldFileName, string path)
        {
            string accountName = this.configuration.GetConnectionString("AccountName");
            string accountKey = this.configuration.GetConnectionString("AccountKey");
            string containerName = this.configuration.GetConnectionString("ContainerName");

            var serviceUri = new Uri($"https://{accountName}.blob.core.windows.net");
            var credential = new StorageSharedKeyCredential(accountName, accountKey);

            var blobServiceClient = new BlobServiceClient(serviceUri, credential);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await blobContainerClient.CreateIfNotExistsAsync();

            var blobCopyClient = blobContainerClient.GetBlobClient($"{path}{newFileName}");

            if (!await blobCopyClient.ExistsAsync())
            {
                var blobClient = blobContainerClient.GetBlobClient(oldFileName);

                if (await blobClient.ExistsAsync())
                {
                    await blobCopyClient.StartCopyFromUriAsync(blobClient.Uri);
                    await blobClient.DeleteIfExistsAsync();
                }
            }

            return Uri.UnescapeDataString(blobCopyClient.Uri.AbsoluteUri);
        }
    }
}
