namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;

    using static Bookworm.Common.Authors.AuthorsDataConstants;

    public class EditBookService : IEditBookService
    {
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IDeletableEntityRepository<Book> booksRepository;
        private readonly IDeletableEntityRepository<Author> authorsRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;

        public EditBookService(
            IDeletableEntityRepository<Publisher> publishersRepository,
            IDeletableEntityRepository<Book> booksRepository,
            IDeletableEntityRepository<Author> authorsRepository,
            IRepository<AuthorBook> authorsBooksRepository)
        {
            this.publishersRepository = publishersRepository;
            this.booksRepository = booksRepository;
            this.authorsRepository = authorsRepository;
            this.authorsBooksRepository = authorsBooksRepository;
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
            IEnumerable<string> authors)
        {
            if (authors.Any() == false)
            {
                throw new Exception("You must add at least one author!");
            }

            foreach (string authorName in authors)
            {
                if (authorName.Length < AuthorNameMinLength || authorName.Length > AuthorNameMaxLength)
                {
                    throw new Exception("Author's name must be between 2 and 50 characters long!");
                }
            }

            int publisherId = 0;
            if (publisherName != null)
            {
                var publisher = this.publishersRepository.All().FirstOrDefault(x => x.Name == publisherName);
                if (publisher == null)
                {
                    publisher = new Publisher() { Name = publisherName };
                    await this.publishersRepository.AddAsync(publisher);
                    await this.publishersRepository.SaveChangesAsync();
                }

                publisherId = publisher.Id;
            }

            var book = this.booksRepository.All().First(x => x.Id == bookId);

            book.Title = title;
            book.Description = description;
            book.CategoryId = categoryId;
            book.LanguageId = languageId;
            book.PagesCount = pagesCount;
            book.Year = publishedYear;
            book.PublisherId = publisherId;

            var authorBooks = this.authorsBooksRepository.All().Where(x => x.BookId == bookId).ToList();
            foreach (var authorBook in authorBooks)
            {
                this.authorsBooksRepository.Delete(authorBook);
            }

            await this.authorsBooksRepository.SaveChangesAsync();

            var authorIds = new List<int>();

            foreach (string authorName in authors)
            {
                var author = this.authorsRepository
                    .AllAsNoTracking()
                    .FirstOrDefault(x => x.Name == authorName);

                if (author == null)
                {
                    author = new Author() { Name = authorName };
                    await this.authorsRepository.AddAsync(author);
                    await this.authorsRepository.SaveChangesAsync();
                }

                authorIds.Add(author.Id);
            }

            foreach (var id in authorIds)
            {
                var authorBook = new AuthorBook() { AuthorId = id, BookId = bookId };
                await this.authorsBooksRepository.AddAsync(authorBook);
            }

            await this.authorsBooksRepository.SaveChangesAsync();
            this.booksRepository.Update(book);
            await this.booksRepository.SaveChangesAsync();
        }
    }
}
