namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.DTOs;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;

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
            this.booksRepository = booksRepository;

            this.blobService = blobService;
            this.authorsService = authorsService;
            this.publishersService = publishersService;
            this.validateBookService = validateBookService;
        }

        public async Task<OperationResult> UploadBookAsync(
            BookDto bookDto,
            string userId)
        {
            var bookTitle = bookDto.Title.Trim();

            await this.validateBookService.ValidateAsync(
                bookTitle,
                bookDto.LanguageId,
                bookDto.CategoryId);

            string bookBlobUrl = await this.blobService.UploadBlobAsync(
                bookDto.BookFile,
                BookFileUploadPath);

            string imageBlobUrl = await this.blobService.UploadBlobAsync(
                bookDto.ImageFile,
                BookImageFileUploadPath);

            var book = new Book
            {
                UserId = userId,
                Title = bookTitle,
                FileUrl = bookBlobUrl,
                ImageUrl = imageBlobUrl,
                Year = bookDto.Year,
                PagesCount = bookDto.PagesCount,
                LanguageId = bookDto.LanguageId,
                CategoryId = bookDto.CategoryId,
                Description = bookDto.Description,
            };

            if (!string.IsNullOrWhiteSpace(bookDto.Publisher))
            {
                var publisherName = bookDto.Publisher.Trim();

                var result = await this.publishersService
                    .GetPublisherWithNameAsync(publisherName);

                if (result.Data == null)
                {
                    book.Publisher = new Publisher { Name = publisherName };
                }
                else
                {
                    book.PublisherId = result.Data.Id;
                }
            }

            var authorsNames = bookDto.Authors.Select(x => x.Name.Trim()).ToList();

            foreach (var authorName in authorsNames)
            {
                var result = await this.authorsService.GetAuthorWithNameAsync(authorName);

                book.AuthorsBooks.Add(result.Data != null ?
                    new AuthorBook { AuthorId = result.Data.Id } :
                    new AuthorBook { Author = new Author { Name = authorName } });
            }

            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();

            return OperationResult.Ok(UploadSuccess);
        }
    }
}
