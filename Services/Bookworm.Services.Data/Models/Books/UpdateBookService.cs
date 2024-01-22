namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.GlobalConstants;
    using static Bookworm.Common.UsersPointsDataConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IRepository<Author> authorsRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;
        private readonly IBlobService blobService;
        private readonly IValidateUploadedBookService validateUploadedBookService;
        private readonly UserManager<ApplicationUser> userManager;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IDeletableEntityRepository<Publisher> publishersRepository,
            IRepository<Author> authorsRepository,
            IRepository<AuthorBook> authorsBooksRepository,
            IBlobService blobService,
            IValidateUploadedBookService validateUploadedBookService,
            UserManager<ApplicationUser> userManager)
        {
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.publishersRepository = publishersRepository;
            this.authorsRepository = authorsRepository;
            this.authorsBooksRepository = authorsBooksRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
            this.userManager = userManager;
        }

        public async Task ApproveBookAsync(string bookId)
        {
            Book book = await this.bookRepository
                .All()
                .FirstAsync(x => x.Id == bookId);
            book.IsApproved = true;
            await this.bookRepository.SaveChangesAsync();

            ApplicationUser user = await this.userRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == book.UserId);
            user.Points += BookPoints;

            this.userRepository.Update(user);
            await this.userRepository.SaveChangesAsync();
        }

        public async Task UnapproveBookAsync(string bookId)
        {
            Book book = await this.bookRepository
                .All()
                .FirstAsync(x => x.Id == bookId);
            book.IsApproved = false;
            await this.bookRepository.SaveChangesAsync();

            ApplicationUser user = await this.userRepository
                .All()
                .FirstAsync(x => x.Id == book.UserId);
            if (user.Points > 0)
            {
                user.Points -= BookPoints;
            }

            this.userRepository.Update(user);
            await this.userRepository.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(string bookId, string userId)
        {
            Book book = await this.bookRepository
                .All()
                .FirstAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException("No book with given id found!");

            ApplicationUser user = await this.userRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == userId) ??
                throw new InvalidOperationException("No user with given id found!");

            bool isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);
            if (book.UserId != user.Id && !isAdmin)
            {
                throw new InvalidOperationException("You have to be either the book's owner or an administrator to delete it!");
            }

            if (user.Points - BookPoints < 0)
            {
                user.Points = 0;
            }
            else
            {
                user.Points -= BookPoints;
            }

            book.IsApproved = false;

            this.userRepository.Update(user);
            await this.userRepository.SaveChangesAsync();

            this.bookRepository.Delete(book);
            await this.bookRepository.SaveChangesAsync();
        }

        public async Task UndeleteBookAsync(string bookId)
        {
            Book book = await this.bookRepository
                .AllWithDeleted()
                .FirstAsync(x => x.Id == bookId);
            this.bookRepository.Undelete(book);
            await this.bookRepository.SaveChangesAsync();
        }

        public async Task EditBookAsync(
            BookDto editBookDto,
            IEnumerable<UploadAuthorViewModel> authors,
            string userId)
        {
            Book book = await this.bookRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == editBookDto.Id) ??
                throw new InvalidOperationException("No book with given id found!");

            if (book.UserId != userId)
            {
                throw new InvalidOperationException("You have to be the book's owner to edit it!");
            }

            await this.validateUploadedBookService.ValidateUploadedBookAsync(
                editBookDto.BookFile,
                editBookDto.ImageFile,
                authors,
                editBookDto.CategoryId,
                editBookDto.LanguageId);

            if (editBookDto.BookFile != null)
            {
                string bookBlobName = book.FileUrl[book.FileUrl.IndexOf("Books") ..];
                book.FileUrl = await this.blobService.ReplaceBlobAsync(
                    editBookDto.BookFile,
                    bookBlobName,
                    BookFileUploadPath);
            }

            if (editBookDto.ImageFile != null)
            {
                string imageBlobName = book.ImageUrl[book.ImageUrl.IndexOf("BooksImages") ..];
                book.ImageUrl = await this.blobService.ReplaceBlobAsync(
                    editBookDto.ImageFile,
                    imageBlobName,
                    BookImageFileUploadPath);
            }

            if (editBookDto.Publisher != null)
            {
                Publisher publisher = await this.publishersRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == editBookDto.Publisher.ToLower());

                if (publisher == null)
                {
                    publisher = new Publisher() { Name = editBookDto.Publisher };
                    await this.publishersRepository.AddAsync(publisher);
                    await this.publishersRepository.SaveChangesAsync();
                }

                book.PublisherId = publisher.Id;
            }

            book.Title = editBookDto.Title;
            book.Description = editBookDto.Description;
            book.CategoryId = editBookDto.CategoryId;
            book.LanguageId = editBookDto.LanguageId;
            book.PagesCount = editBookDto.PagesCount;
            book.Year = editBookDto.PublishedYear;

            List<AuthorBook> authorBooks = await this.authorsBooksRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == book.Id)
                .ToListAsync();

            foreach (AuthorBook authorBook in authorBooks)
            {
                Author author = await this.authorsRepository
                    .AllAsNoTracking()
                    .FirstAsync(x => x.Id == authorBook.AuthorId);

                bool authorBookIsValid = authors.Any(x =>
                    x.Name.Trim().ToLower() == author.Name.ToLower());

                if (!authorBookIsValid)
                {
                    this.authorsBooksRepository.Delete(authorBook);
                }
            }

            await this.authorsBooksRepository.SaveChangesAsync();

            foreach (UploadAuthorViewModel inputAuthor in authors)
            {
                Author author = await this.authorsRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Name.ToLower() == inputAuthor.Name.Trim().ToLower());

                if (author == null)
                {
                    author = new Author() { Name = inputAuthor.Name.Trim() };
                    await this.authorsRepository.AddAsync(author);
                    await this.authorsRepository.SaveChangesAsync();

                    AuthorBook authorBook = new AuthorBook()
                    {
                        AuthorId = author.Id,
                        BookId = book.Id,
                    };
                    await this.authorsBooksRepository.AddAsync(authorBook);
                    await this.authorsBooksRepository.SaveChangesAsync();
                }
                else
                {
                    AuthorBook authorBook = await this.authorsBooksRepository
                        .AllAsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.BookId == book.Id && x.AuthorId == author.Id);

                    if (authorBook == null)
                    {
                        authorBook = new AuthorBook()
                        {
                            AuthorId = author.Id,
                            BookId = book.Id,
                        };
                        await this.authorsBooksRepository.AddAsync(authorBook);
                        await this.authorsBooksRepository.SaveChangesAsync();
                    }
                }
            }

            book.IsApproved = false;
            ApplicationUser user = await this.userRepository.All().FirstAsync(x => x.Id == userId);
            if (user.Points - BookPoints < 0)
            {
                user.Points = 0;
            }
            else if (user.Points > 0)
            {
                user.Points -= BookPoints;
            }

            this.userRepository.Update(user);
            await this.userRepository.SaveChangesAsync();

            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();
        }
    }
}
