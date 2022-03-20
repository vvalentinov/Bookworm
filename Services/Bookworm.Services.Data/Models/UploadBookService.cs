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

    using static Bookworm.Common.DataConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IConfiguration configuration;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IDeletableEntityRepository<Publisher> publisherRepository;
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly IBlobService blobService;

        public UploadBookService(
            IConfiguration configuration,
            IDeletableEntityRepository<Book> booksRepository,
            IDeletableEntityRepository<Publisher> publisherRepository,
            IDeletableEntityRepository<Author> authorRepository,
            IBlobService blobService)
        {
            this.configuration = configuration;
            this.booksRepository = booksRepository;
            this.publisherRepository = publisherRepository;
            this.authorRepository = authorRepository;
            this.blobService = blobService;
        }

        public async Task UploadBookAsync(
            string title,
            string description,
            int languageId,
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

            if (await this.blobService.CheckIfBlobExistsAsync(bookFile.FileName))
            {
                throw new Exception("Please, try changing the PDF file name!");
            }
            else if (await this.blobService.CheckIfBlobExistsAsync(imageFile.FileName))
            {
                throw new Exception("Please, try changing the image file name!");
            }

            await this.blobService.UploadBlobAsync(bookFile);
            await this.blobService.UploadBlobAsync(imageFile);

            string bookFileBlobUrl = this.blobService.GetBlobAbsoluteUri(bookFile.FileName);
            string imageFileBlobUrl = this.blobService.GetBlobAbsoluteUri(imageFile.FileName);

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

            if (publisher != null)
            {
                Publisher bookPublisher = this.publisherRepository
               .AllAsNoTracking()
               .FirstOrDefault(x => x.Name == publisher);

                if (bookPublisher == null)
                {
                    bookPublisher = new Publisher() { Name = publisher };
                    bookPublisher.Books.Add(book);
                    await this.publisherRepository.AddAsync(bookPublisher);
                    await this.publisherRepository.SaveChangesAsync();
                }

                book.Publisher = bookPublisher;
                this.booksRepository.Update(book);
            }

            List<AuthorBook> bookAuthors = new List<AuthorBook>();
            foreach (string author in authors)
            {
                Author bookAauthor = this.authorRepository.AllAsNoTracking().FirstOrDefault(x => x.Name == author);
                if (bookAauthor == null)
                {
                    bookAauthor = new Author() { Name = author };
                }

                AuthorBook authorBook = new AuthorBook() { Book = book, Author = bookAauthor };
                bookAuthors.Add(authorBook);
            }

            book.AuthorsBooks = bookAuthors;
            this.booksRepository.Update(book);
            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
