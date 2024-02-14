namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;

    public class UploadBookService : IUploadBookService
    {
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IRepository<Publisher> publisherRepository;
        private readonly IRepository<Author> authorRepository;
        private readonly IRepository<AuthorBook> authorBookRepository;
        private readonly IBlobService blobService;
        private readonly IValidateUploadedBookService validateUploadedBookService;

        public UploadBookService(
            IDeletableEntityRepository<Book> booksRepository,
            IRepository<Publisher> publisherRepository,
            IRepository<Author> authorRepository,
            IRepository<AuthorBook> authorBookRepository,
            IBlobService blobService,
            IValidateUploadedBookService validateUploadedBookService)
        {
            this.booksRepository = booksRepository;
            this.publisherRepository = publisherRepository;
            this.authorRepository = authorRepository;
            this.authorBookRepository = authorBookRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
        }

        public async Task UploadBookAsync(
            BookDto uploadBookDto,
            ICollection<UploadAuthorViewModel> authors,
            string userId)
        {
            bool bookWithTitleExist = await this.booksRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Title.ToLower() == uploadBookDto.Title.Trim().ToLower());

            if (bookWithTitleExist)
            {
                throw new InvalidOperationException("Book with given name already exist!");
            }

            await this.validateUploadedBookService.ValidateUploadedBookAsync(
                uploadBookDto.BookFile,
                uploadBookDto.ImageFile,
                authors,
                uploadBookDto.CategoryId,
                uploadBookDto.LanguageId);

            string bookBlobName = await this.blobService.UploadBlobAsync(
                uploadBookDto.BookFile,
                BookFileUploadPath);
            string imageBlobName = await this.blobService.UploadBlobAsync(
                uploadBookDto.ImageFile,
                BookImageFileUploadPath);

            string bookFileBlobUrl = this.blobService.GetBlobAbsoluteUri(bookBlobName);
            string bookImageFileBlobUrl = this.blobService.GetBlobAbsoluteUri(imageBlobName);

            var book = new Book
            {
                UserId = userId,
                FileUrl = bookFileBlobUrl,
                Title = uploadBookDto.Title.Trim(),
                ImageUrl = bookImageFileBlobUrl,
                Year = uploadBookDto.PublishedYear,
                LanguageId = uploadBookDto.LanguageId,
                CategoryId = uploadBookDto.CategoryId,
                PagesCount = uploadBookDto.PagesCount,
                Description = uploadBookDto.Description,
            };

            if (!string.IsNullOrWhiteSpace(uploadBookDto.Publisher))
            {
                var publisher = await this.publisherRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == uploadBookDto.Publisher.ToLower());

                if (publisher == null)
                {
                    publisher = new Publisher() { Name = uploadBookDto.Publisher.Trim() };
                    await this.publisherRepository.AddAsync(publisher);
                    await this.publisherRepository.SaveChangesAsync();
                }

                book.PublisherId = publisher.Id;
            }

            await this.booksRepository.AddAsync(book);
            await this.booksRepository.SaveChangesAsync();

            foreach (var authorModel in authors)
            {
                var author = await this.authorRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == authorModel.Name.Trim().ToLower());

                if (author == null)
                {
                    author = new Author() { Name = authorModel.Name.Trim() };
                    await this.authorRepository.AddAsync(author);
                    await this.authorRepository.SaveChangesAsync();

                    var authorBook = new AuthorBook
                    {
                        BookId = book.Id,
                        AuthorId = author.Id,
                    };

                    await this.authorBookRepository.AddAsync(authorBook);
                    await this.authorBookRepository.SaveChangesAsync();
                }
                else
                {
                    var authorBookConnection = await this.authorBookRepository
                        .AllAsNoTracking()
                        .FirstOrDefaultAsync(x => x.BookId == book.Id && x.AuthorId == author.Id);

                    if (authorBookConnection == null)
                    {
                        var authorBook = new AuthorBook
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
    }
}
