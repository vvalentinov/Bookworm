namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using static Bookworm.Common.DataConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IConfiguration configuration;
        private readonly IRepository<Language> languagesRepository;
        private readonly IDeletableEntityRepository<Book> booksRepository;

        public UploadBookService(
            IConfiguration configuration,
            IRepository<Language> languagesRepository,
            IDeletableEntityRepository<Book> booksRepository)
        {
            this.configuration = configuration;
            this.languagesRepository = languagesRepository;
            this.booksRepository = booksRepository;
        }

        public async Task UploadBookAsync(
            string title,
            string description,
            string language,
            string publisher,
            int pagesCount,
            int publishedYear,
            IFormFile bookFile,
            IFormFile imageFile,
            int categoryId,
            IEnumerable<string> authors,
            string userId)
        {
            if (bookFile == null)
            {
                throw new Exception("PDF file field is empty!");
            }

            if (imageFile == null)
            {
                throw new Exception("Image file field is empty!");
            }

            string[] permittedImageExtensions = { ".png", ".jpg", ".jpeg" };
            string bookFileExtension = Path.GetExtension(bookFile.FileName);
            string bookImageExtension = Path.GetExtension(imageFile.FileName);

            if (bookFileExtension != ".pdf")
            {
                throw new Exception("Must be in PDF format!");
            }

            if (permittedImageExtensions.Contains(bookImageExtension) == false)
            {
                throw new Exception("Valid formats are: JPG, JPEG and PNG!");
            }

            if (authors.Any() == false)
            {
                throw new Exception("You must add at least one author!");
            }

            foreach (string authorName in authors)
            {
                if (authorName.Length < AuthorNameMin || authorName.Length > AuthorNameMax)
                {
                    throw new Exception("Author's name must be between 2 and 50 characters long!");
                }
            }

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.configuration.GetConnectionString("StorageConnection"));
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(this.configuration.GetConnectionString("ContainerName"));

            CloudBlockBlob bookFileBlob = cloudBlobContainer.GetBlockBlobReference(bookFile.FileName);

            if (await bookFileBlob.ExistsAsync())
            {
                throw new Exception("Please, try changing the PDF file name!");
            }

            CloudBlockBlob imageFileBlob = cloudBlobContainer.GetBlockBlobReference(imageFile.FileName);

            if (await imageFileBlob.ExistsAsync())
            {
                throw new Exception("Please, try changing the image file name!");
            }

            string bookFileBlobUrl = bookFileBlob.Uri.AbsoluteUri;
            string imageFileBlobUrl = imageFileBlob.Uri.AbsoluteUri;

            int languageId = this.languagesRepository.AllAsNoTracking().First(x => x.Name == language).Id;
            Publisher bookPublisher = new Publisher() { Name = publisher };

            Book book = new Book()
            {
                Title = title,
                LanguageId = languageId,
                Description = description,
                PagesCount = pagesCount,
                Year = publishedYear,
                CategoryId = categoryId,
                UserId = userId,
                FileUrl = bookFileBlobUrl,
                ImageUrl = imageFileBlobUrl,
            };

            PublisherBook publisherBook = new PublisherBook() { BookId = book.Id, PublisherId = bookPublisher.Id };
            book.PublishersBooks.Add(publisherBook);

            List<AuthorBook> bookAuthors = new List<AuthorBook>();
            foreach (string author in authors)
            {
                Author bookAauthor = new Author() { Name = author };
                AuthorBook authorBook = new AuthorBook() { Book = book, Author = bookAauthor };
                bookAuthors.Add(authorBook);
            }

            book.AuthorsBooks = bookAuthors;

            bookFileBlob.Properties.ContentType = bookFile.ContentType;
            imageFileBlob.Properties.ContentType = imageFile.ContentType;

            await bookFileBlob.UploadFromStreamAsync(bookFile.OpenReadStream());
            await imageFileBlob.UploadFromStreamAsync(imageFile.OpenReadStream());

            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
