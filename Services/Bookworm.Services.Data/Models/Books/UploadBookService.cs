namespace Bookworm.Services.Data.Models.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IDeletableEntityRepository<Publisher> publisherRepository;
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly IRepository<AuthorBook> authorBookRepository;
        private readonly IBlobService blobService;

        public UploadBookService(
            IDeletableEntityRepository<Book> booksRepository,
            IDeletableEntityRepository<Publisher> publisherRepository,
            IDeletableEntityRepository<Author> authorRepository,
            IRepository<AuthorBook> authorBookRepository,
            IBlobService blobService)
        {
            this.booksRepository = booksRepository;
            this.publisherRepository = publisherRepository;
            this.authorRepository = authorRepository;
            this.authorBookRepository = authorBookRepository;
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
            string userId,
            string userName)
        {
            await this.blobService.UploadBlobAsync(bookFile, BookFileUploadPath);
            await this.blobService.UploadBlobAsync(imageFile, BookImageFileUploadPath);

            string bookFileBlobUrl = this.blobService.GetBlobAbsoluteUri($"{BookFileUploadPath}{bookFile.FileName}");
            string bookImageFileBlobUrl = this.blobService.GetBlobAbsoluteUri($"{BookImageFileUploadPath}{imageFile.FileName}");

            Book book = new Book
            {
                Title = title,
                LanguageId = languageId,
                Description = description,
                PagesCount = pagesCount,
                Year = publishedYear,
                CategoryId = categoryId,
                UserId = userId,
                FileUrl = bookFileBlobUrl,
                ImageUrl = bookImageFileBlobUrl,
            };

            if (string.IsNullOrWhiteSpace(publisher) == false)
            {
                Publisher bookPublisher = await this.publisherRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Name == publisher);

                if (bookPublisher == null)
                {
                    bookPublisher = new Publisher() { Name = publisher };
                    await this.publisherRepository.AddAsync(bookPublisher);
                    await this.publisherRepository.SaveChangesAsync();
                }

                book.PublisherId = bookPublisher.Id;
            }

            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();

            foreach (string authorName in authors)
            {
                Author author = await this.authorRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Name == authorName);

                if (author == null)
                {
                    author = new Author() { Name = authorName };
                    await this.authorRepository.AddAsync(author);
                    await this.authorRepository.SaveChangesAsync();
                }

                AuthorBook authorBook = new AuthorBook()
                {
                    BookId = book.Id,
                    AuthorId = author.Id,
                };

                await this.authorBookRepository.AddAsync(authorBook);
                await this.authorBookRepository.SaveChangesAsync();
            }
        }
    }
}
