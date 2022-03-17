namespace Bookworm.Services.Data.Models
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class DownloadBookService : IDownloadBookService
    {
        private readonly IConfiguration configuration;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public DownloadBookService(IConfiguration configuration, IDeletableEntityRepository<Book> bookRepository)
        {
            this.configuration = configuration;
            this.bookRepository = bookRepository;
        }

        public async Task<Tuple<Stream, string, string>> DownloadAsync(string bookId)
        {
            Book book = this.bookRepository.AllAsNoTracking().FirstOrDefault(x => x.Id == bookId);
            book.DownloadsCount++;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();
            Uri uri = new Uri(book.FileUrl);
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.configuration.GetConnectionString("StorageConnection"));
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(uri, blobClient);
            MemoryStream ms = new MemoryStream();
            await cloudBlockBlob.DownloadToStreamAsync(ms);
            Stream blobStream = cloudBlockBlob.OpenReadAsync().Result;
            return Tuple.Create(blobStream, cloudBlockBlob.Properties.ContentType, cloudBlockBlob.Name);
        }
    }
}
