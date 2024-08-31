namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Contracts;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.DTOs;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IBlobService blobService;
        private readonly IAuthorsService authorsService;
        private readonly IPublishersService publishersService;
        private readonly IValidateBookService validateBookService;
        private readonly IDeletableEntityRepository<Book> booksRepository;

        public UploadBookService(
            IBlobService blobService,
            IAuthorsService authorsService,
            IPublishersService publishersService,
            IValidateBookService validateBookService,
            IDeletableEntityRepository<Book> booksRepository)
        {
            this.blobService = blobService;
            this.authorsService = authorsService;
            this.booksRepository = booksRepository;
            this.publishersService = publishersService;
            this.validateBookService = validateBookService;
        }

        public async Task UploadBookAsync(BookDto uploadBookDto, string userId)
        {
            var bookTitle = uploadBookDto.Title.Trim();

            await this.validateBookService.ValidateAsync(
                bookTitle,
                uploadBookDto.LanguageId,
                uploadBookDto.CategoryId);

            string bookBlobUrl = await this.blobService.UploadBlobAsync(uploadBookDto.BookFile, BookFileUploadPath);
            string imageBlobUrl = await this.blobService.UploadBlobAsync(uploadBookDto.ImageFile, BookImageFileUploadPath);

            var book = new Book
            {
                UserId = userId,
                Title = bookTitle,
                FileUrl = bookBlobUrl,
                ImageUrl = imageBlobUrl,
                Year = uploadBookDto.Year,
                PagesCount = uploadBookDto.PagesCount,
                LanguageId = uploadBookDto.LanguageId,
                CategoryId = uploadBookDto.CategoryId,
                Description = uploadBookDto.Description,
            };

            if (!string.IsNullOrWhiteSpace(uploadBookDto.Publisher))
            {
                var publisherName = uploadBookDto.Publisher.Trim();

                var publisher = await this.publishersService.GetPublisherWithNameAsync(publisherName);

                if (publisher == null)
                {
                    book.Publisher = new Publisher { Name = publisherName };
                }
                else
                {
                    book.PublisherId = publisher.Id;
                }
            }

            var authorsNames = uploadBookDto.Authors.Select(x => x.Name.Trim()).ToList();

            foreach (var authorName in authorsNames)
            {
                var author = await this.authorsService.GetAuthorWithNameAsync(authorName);

                book.AuthorsBooks.Add(author != null ?
                    new AuthorBook { AuthorId = author.Id } :
                    new AuthorBook { Author = new Author { Name = authorName } });
            }

            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
