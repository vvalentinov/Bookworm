namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.DTOs;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IBlobService blobService;
        private readonly IAuthorsService authorsService;
        private readonly IPublishersService publishersService;
        private readonly ISearchBooksService searchBooksService;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IValidateUploadedBookService validateUploadedBookService;

        public UploadBookService(
            IBlobService blobService,
            IAuthorsService authorsService,
            IPublishersService publishersService,
            ISearchBooksService searchBooksService,
            IDeletableEntityRepository<Book> booksRepository,
            IValidateUploadedBookService validateUploadedBookService)
        {
            this.blobService = blobService;
            this.authorsService = authorsService;
            this.booksRepository = booksRepository;
            this.publishersService = publishersService;
            this.searchBooksService = searchBooksService;
            this.validateUploadedBookService = validateUploadedBookService;
        }

        public async Task UploadBookAsync(BookDto uploadBookDto)
        {
            var bookExists = await this.searchBooksService
                .CheckIfBookWithTitleExistsAsync(uploadBookDto.Title);
            if (bookExists)
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
                Title = uploadBookDto.Title.Trim(),
                FileUrl = bookBlobUrl,
                ImageUrl = imageBlobUrl,
                Year = uploadBookDto.Year,
                UserId = uploadBookDto.BookCreatorId,
                PagesCount = uploadBookDto.PagesCount,
                LanguageId = uploadBookDto.LanguageId,
                CategoryId = uploadBookDto.CategoryId,
                Description = uploadBookDto.Description,
            };

            await this.booksRepository.AddAsync(book);

            if (!string.IsNullOrWhiteSpace(uploadBookDto.Publisher))
            {
                var publisher = await this.publishersService
                    .GetPublisherWithNameAsync(uploadBookDto.Publisher);

                if (publisher == null)
                {
                    book.Publisher = new Publisher
                    {
                        Name = uploadBookDto.Publisher.Trim(),
                    };
                }
                else
                {
                    book.PublisherId = publisher.Id;
                }
            }

            foreach (var authorModel in uploadBookDto.Authors)
            {
                var author = await this.authorsService
                    .GetAuthorWithNameAsync(authorModel.Name);

                if (author == null)
                {
                    book.AuthorsBooks.Add(new AuthorBook
                    {
                        Book = book,
                        Author = new Author { Name = authorModel.Name.Trim() },
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
