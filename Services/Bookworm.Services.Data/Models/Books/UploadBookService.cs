namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IBlobService blobService;
        private readonly IRepository<Author> authorRepository;
        private readonly IRepository<Publisher> publisherRepository;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IValidateUploadedBookService validateUploadedBookService;

        public UploadBookService(
            IBlobService blobService,
            IRepository<Author> authorRepository,
            IRepository<Publisher> publisherRepository,
            IDeletableEntityRepository<Book> booksRepository,
            IValidateUploadedBookService validateUploadedBookService)
        {
            this.booksRepository = booksRepository;
            this.publisherRepository = publisherRepository;
            this.authorRepository = authorRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
        }

        public async Task UploadBookAsync(BookDto uploadBookDto)
        {
            var bookTitle = uploadBookDto.Title.Trim();

            bool bookWithTitleExist = await this.booksRepository
                .AllAsNoTrackingWithDeleted()
                .AnyAsync(x => x.Title.ToLower() == bookTitle.ToLower());

            if (bookWithTitleExist)
            {
                throw new InvalidOperationException(BookWithTitleExistsError);
            }

            await this.validateUploadedBookService.ValidateUploadedBookAsync(
                false,
                uploadBookDto.CategoryId,
                uploadBookDto.LanguageId,
                uploadBookDto.BookFile,
                uploadBookDto.ImageFile,
                uploadBookDto.Authors);

            string bookBlobUrl = await this.blobService.UploadBlobAsync(
                uploadBookDto.BookFile,
                BookFileUploadPath);

            string imageBlobUrl = await this.blobService.UploadBlobAsync(
                uploadBookDto.ImageFile,
                BookImageFileUploadPath);

            var book = new Book
            {
                Title = bookTitle,
                FileUrl = bookBlobUrl,
                ImageUrl = imageBlobUrl,
                Year = uploadBookDto.PublishedYear,
                UserId = uploadBookDto.BookCreatorId,
                PagesCount = uploadBookDto.PagesCount,
                LanguageId = uploadBookDto.LanguageId,
                CategoryId = uploadBookDto.CategoryId,
                Description = uploadBookDto.Description,
            };

            await this.booksRepository.AddAsync(book);

            if (!string.IsNullOrWhiteSpace(uploadBookDto.Publisher))
            {
                var publisherName = uploadBookDto.Publisher.Trim();

                var publisher = await this.publisherRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == publisherName.ToLower());

                if (publisher == null)
                {
                    book.Publisher = new Publisher { Name = publisherName };
                }
                else
                {
                    book.PublisherId = publisher.Id;
                }
            }

            foreach (var authorModel in uploadBookDto.Authors)
            {
                var authorName = authorModel.Name.Trim();

                var author = await this.authorRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == authorName.ToLower());

                if (author == null)
                {
                    book.AuthorsBooks.Add(new AuthorBook
                    {
                        Book = book,
                        Author = new Author { Name = authorName },
                    });
                }
                else
                {
                    book.AuthorsBooks.Add(new AuthorBook { Author = author, Book = book });
                }
            }

            await this.booksRepository.SaveChangesAsync();
        }
    }
}
