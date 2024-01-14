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

    using static Bookworm.Common.UsersPointsDataConstants;

    public class UpdateBookService : IUpdateBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IDeletableEntityRepository<Author> authorsRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;

        public UpdateBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IDeletableEntityRepository<Publisher> publishersRepository,
            IDeletableEntityRepository<Book> booksRepository,
            IDeletableEntityRepository<Author> authorsRepository,
            IRepository<AuthorBook> authorsBooksRepository)
        {
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.publishersRepository = publishersRepository;
            this.booksRepository = booksRepository;
            this.authorsRepository = authorsRepository;
            this.authorsBooksRepository = authorsBooksRepository;
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

        public async Task DeleteBookAsync(string bookId)
        {
            Book book = await this.bookRepository
                .All()
                .FirstAsync(x => x.Id == bookId);
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
            bool hasDuplicates = authors
                .Select(x => x.Name)
                .GroupBy(author => author)
                .Any(group => group.Count() > 1);

            if (hasDuplicates)
            {
                throw new Exception("No author duplicates allowed!");
            }

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
                .Where(x => x.BookId == book.Id)
                .ToListAsync();

            this.authorsBooksRepository.RemoveRange(authorBooks);
            await this.authorsBooksRepository.SaveChangesAsync();

            foreach (UploadAuthorViewModel inputAuthor in authors)
            {
                Author author = await this.authorsRepository
                    .All()
                    .FirstOrDefaultAsync(x => x.Name == inputAuthor.Name);

                if (author == null)
                {
                    var newAuthor = new Author() { Name = inputAuthor.Name };
                    await this.authorsRepository.AddAsync(newAuthor);
                    await this.authorsRepository.SaveChangesAsync();

                    var authorBook = new AuthorBook() { AuthorId = newAuthor.Id, BookId = book.Id };
                    await this.authorsBooksRepository.AddAsync(authorBook);
                    await this.authorsBooksRepository.SaveChangesAsync();
                }
                else
                {
                    bool authorBookExists = await this.authorsBooksRepository
                        .AllAsNoTracking()
                        .AnyAsync(x => x.AuthorId == author.Id && x.BookId == book.Id);

                    if (authorBookExists == false)
                    {
                        var authorBook = new AuthorBook() { AuthorId = author.Id, BookId = book.Id };
                        await this.authorsBooksRepository.AddAsync(authorBook);
                        await this.authorsBooksRepository.SaveChangesAsync();
                    }
                }
            }

            this.booksRepository.Update(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
