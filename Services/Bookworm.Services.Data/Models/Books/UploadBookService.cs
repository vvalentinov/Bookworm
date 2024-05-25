namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.DTOs;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.AuthorErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.LanguageErrorMessagesConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IBlobService blobService;
        private readonly IAuthorsService authorsService;
        private readonly IPublishersService publishersService;
        private readonly ISearchBooksService searchBooksService;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IValidateBookFilesSizesService validateUploadedBookService;
        private readonly ICategoriesService categoriesService;
        private readonly ILanguagesService languagesService;

        public UploadBookService(
            IBlobService blobService,
            IAuthorsService authorsService,
            IPublishersService publishersService,
            ISearchBooksService searchBooksService,
            IDeletableEntityRepository<Book> booksRepository,
            IValidateBookFilesSizesService validateUploadedBookService,
            ICategoriesService categoriesService,
            ILanguagesService languagesService)
        {
            this.blobService = blobService;
            this.authorsService = authorsService;
            this.booksRepository = booksRepository;
            this.publishersService = publishersService;
            this.searchBooksService = searchBooksService;
            this.validateUploadedBookService = validateUploadedBookService;
            this.categoriesService = categoriesService;
            this.languagesService = languagesService;
        }

        public async Task UploadBookAsync(BookDto uploadBookDto)
        {
            var bookTitle = uploadBookDto.Title.Trim();
            var publisherName = uploadBookDto.Publisher.Trim();

            if (await this.searchBooksService.CheckIfBookWithTitleExistsAsync(bookTitle))
            {
                throw new InvalidOperationException(BookWithTitleExistsError);
            }

            if (!await this.categoriesService.CheckIfIdIsValidAsync(uploadBookDto.CategoryId))
            {
                throw new InvalidOperationException(CategoryNotFoundError);
            }

            if (!await this.languagesService.CheckIfIdIsValidAsync(uploadBookDto.LanguageId))
            {
                throw new InvalidOperationException(LanguageNotFoundError);
            }

            if (this.authorsService.HasDuplicates(uploadBookDto.Authors))
            {
                throw new InvalidOperationException(AuthorDuplicatesError);
            }

            this.validateUploadedBookService.ValidateUploadedBookFileSizes(
                isForEdit: false,
                uploadBookDto.BookFile,
                uploadBookDto.ImageFile);

            string bookBlobUrl = await this.blobService.UploadBlobAsync(uploadBookDto.BookFile, BookFileUploadPath);
            string imageBlobUrl = await this.blobService.UploadBlobAsync(uploadBookDto.ImageFile, BookImageFileUploadPath);

            var book = new Book
            {
                Title = bookTitle,
                FileUrl = bookBlobUrl,
                ImageUrl = imageBlobUrl,
                Year = uploadBookDto.Year,
                UserId = uploadBookDto.BookCreatorId,
                PagesCount = uploadBookDto.PagesCount,
                LanguageId = uploadBookDto.LanguageId,
                CategoryId = uploadBookDto.CategoryId,
                Description = uploadBookDto.Description,
            };

            if (!string.IsNullOrWhiteSpace(uploadBookDto.Publisher))
            {
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

            var trimmedAuthorNames = uploadBookDto.Authors.Select(x => x.Name.Trim()).ToList();

            foreach (var authorName in trimmedAuthorNames)
            {
                var author = await this.authorsService.GetAuthorWithNameAsync(authorName);

                book.AuthorsBooks.Add(author != null ?
                    new AuthorBook { Book = book, Author = author } :
                    new AuthorBook { Book = book, Author = new Author { Name = authorName } });
            }

            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
