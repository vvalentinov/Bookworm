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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;
    using static Bookworm.Common.GlobalConstants;
    using static Bookworm.Common.PointsDataConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Publisher> publishersRepository;
        private readonly IRepository<Author> authorsRepository;
        private readonly IBlobService blobService;
        private readonly IValidateUploadedBookService validateUploadedBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService usersService;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Publisher> publishersRepository,
            IRepository<Author> authorsRepository,
            IBlobService blobService,
            IValidateUploadedBookService validateUploadedBookService,
            UserManager<ApplicationUser> userManager,
            IUsersService usersService)
        {
            this.bookRepository = bookRepository;
            this.publishersRepository = publishersRepository;
            this.authorsRepository = authorsRepository;
            this.blobService = blobService;
            this.validateUploadedBookService = validateUploadedBookService;
            this.userManager = userManager;
            this.usersService = usersService;
        }

        public async Task ApproveBookAsync(int bookId)
        {
            var book = await this.bookRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            book.IsApproved = true;

            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            var user = await this.userManager.FindByIdAsync(book.UserId);

            await this.usersService.IncreaseUserPointsAsync(user, BookPoints);
        }

        public async Task UnapproveBookAsync(int bookId)
        {
            var book = await this.bookRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            book.IsApproved = false;

            await this.bookRepository.SaveChangesAsync();

            var user = await this.userManager.FindByIdAsync(book.UserId);

            await this.usersService.ReduceUserPointsAsync(user, BookPoints);
        }

        public async Task DeleteBookAsync(int bookId, string userId)
        {
            var book = await this.bookRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            var user = await this.userManager.FindByIdAsync(userId) ??
                throw new InvalidOperationException("No user with given id found!");

            bool isAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);
            if (book.UserId != user.Id && !isAdmin)
            {
                throw new InvalidOperationException(BookDeleteError);
            }

            await this.usersService.ReduceUserPointsAsync(user, BookPoints);

            book.IsApproved = false;

            this.bookRepository.Delete(book);

            await this.bookRepository.SaveChangesAsync();
        }

        public async Task UndeleteBookAsync(int bookId)
        {
            var book = await this.bookRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            this.bookRepository.Undelete(book);

            await this.bookRepository.SaveChangesAsync();
        }

        public async Task EditBookAsync(BookDto editBookDto, string userId)
        {
            var book = await this.bookRepository
                .All()
                .Include(x => x.Publisher)
                .Include(b => b.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == editBookDto.Id) ??
                throw new InvalidOperationException(BookWrongIdError);

            var user = await this.userManager.FindByIdAsync(userId) ??
                throw new InvalidOperationException("No user with given id found!");

            if (book.UserId != userId)
            {
                throw new InvalidOperationException(BookEditError);
            }

            await this.validateUploadedBookService.ValidateUploadedBookAsync(
                true,
                editBookDto.CategoryId,
                editBookDto.LanguageId,
                editBookDto.BookFile,
                editBookDto.ImageFile,
                editBookDto.Authors);

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

            var publisherName = editBookDto.Publisher.Trim();

            if (editBookDto.Publisher != null && book.Publisher.Name != publisherName)
            {
                var publisher = await this.publishersRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(p => p.Name == publisherName);

                book.Publisher = publisher ?? new Publisher { Name = publisherName };
            }

            book.Title = editBookDto.Title;
            book.Description = editBookDto.Description;
            book.CategoryId = editBookDto.CategoryId;
            book.LanguageId = editBookDto.LanguageId;
            book.PagesCount = editBookDto.PagesCount;
            book.Year = editBookDto.PublishedYear;
            book.IsApproved = false;

            var authorsNames = editBookDto.Authors.Select(x => x.Name.Trim()).ToList();

            book.AuthorsBooks.Clear();

            foreach (var name in authorsNames)
            {
                var author = await this.authorsRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(a => a.Name == name);

                if (author == null)
                {
                    book.AuthorsBooks.Add(new AuthorBook { Author = new Author { Name = name }, Book = book });
                }
                else
                {
                    book.AuthorsBooks.Add(new AuthorBook { Author = author, Book = book });
                }
            }

            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.ReduceUserPointsAsync(user, BookPoints);
        }
    }
}
