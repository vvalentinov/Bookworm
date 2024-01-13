namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Authors.AuthorsDataConstants;
    using static Bookworm.Common.UsersPointsDataConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<UserPoints> usersPointsRepository;
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IDeletableEntityRepository<Author> authorsRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<UserPoints> usersPointsRepository,
            IDeletableEntityRepository<Publisher> publishersRepository,
            IDeletableEntityRepository<Book> booksRepository,
            IDeletableEntityRepository<Author> authorsRepository,
            IRepository<AuthorBook> authorsBooksRepository)
        {
            this.bookRepository = bookRepository;
            this.usersPointsRepository = usersPointsRepository;
            this.publishersRepository = publishersRepository;
            this.booksRepository = booksRepository;
            this.authorsRepository = authorsRepository;
            this.authorsBooksRepository = authorsBooksRepository;
        }

        public async Task ApproveBookAsync(string bookId)
        {
            Book book = await this.bookRepository.All().FirstAsync(x => x.Id == bookId);
            book.IsApproved = true;
            await this.bookRepository.SaveChangesAsync();

            UserPoints user = await this.usersPointsRepository.All().FirstOrDefaultAsync(x => x.UserId == book.UserId);

            if (user == null)
            {
                user = new UserPoints()
                {
                    UserId = book.UserId,
                    Points = BookPoints,
                };
            }
            else
            {
                user.Points += BookPoints;
            }

            await this.usersPointsRepository.SaveChangesAsync();
        }

        public async Task UnapproveBookAsync(string bookId)
        {
            Book book = await this.bookRepository.All().FirstAsync(x => x.Id == bookId);
            book.IsApproved = false;
            await this.bookRepository.SaveChangesAsync();

            UserPoints userPoints = await this.usersPointsRepository.All().FirstAsync(x => x.UserId == book.UserId);
            if (userPoints.Points > 0)
            {
                userPoints.Points -= BookPoints;
            }

            await this.usersPointsRepository.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(string bookId)
        {
            Book book = await this.bookRepository.All().FirstAsync(x => x.Id == bookId);
            this.bookRepository.Delete(book);
            await this.bookRepository.SaveChangesAsync();
        }

        public async Task UndeleteBookAsync(string bookId)
        {
            Book book = await this.bookRepository.AllWithDeleted().FirstAsync(x => x.Id == bookId);
            this.bookRepository.Undelete(book);
            await this.bookRepository.SaveChangesAsync();
        }

        public async Task EditBookAsync(
            string bookId,
            string title,
            string description,
            int categoryId,
            int languageId,
            int pagesCount,
            int publishedYear,
            string publisherName,
            IEnumerable<UploadAuthorViewModel> authors)
        {
            Book book = await this.booksRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new Exception("No book with given id found!");

            if (authors.Any() == false)
            {
                throw new Exception("Authors count must be between one and five!");
            }

            if (publisherName != null)
            {
                Publisher publisher = await this.publishersRepository
                    .All()
                    .FirstOrDefaultAsync(x => x.Name == publisherName);

                if (publisher == null)
                {
                    publisher = new Publisher() { Name = publisherName };
                    await this.publishersRepository.AddAsync(publisher);
                    await this.publishersRepository.SaveChangesAsync();
                }

                book.PublisherId = publisher.Id;
            }

            book.Title = title;
            book.Description = description;
            book.CategoryId = categoryId;
            book.LanguageId = languageId;
            book.PagesCount = pagesCount;
            book.Year = publishedYear;

            List<AuthorBook> authorBooks = await this.authorsBooksRepository
                .All()
                .Where(x => x.BookId == bookId)
                .ToListAsync();

            foreach (AuthorBook authorBook in authorBooks)
            {
                this.authorsBooksRepository.Delete(authorBook);
            }

            await this.authorsBooksRepository.SaveChangesAsync();

            foreach (UploadAuthorViewModel authorModel in authors)
            {
                Author author = await this.authorsRepository
                    .AllAsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == authorModel.Id);

                if (author == null)
                {
                    author = new Author() { Name = authorModel.Name };
                    await this.authorsRepository.AddAsync(author);
                    await this.authorsRepository.SaveChangesAsync();

                    AuthorBook authorBook = new AuthorBook() { AuthorId = author.Id, BookId = book.Id };
                    await this.authorsBooksRepository.AddAsync(authorBook);
                    await this.authorsBooksRepository.SaveChangesAsync();
                }
                else
                {
                    author.Name = authorModel.Name;
                    this.authorsRepository.Update(author);
                    await this.authorsRepository.SaveChangesAsync();
                }
            }

            this.booksRepository.Update(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
