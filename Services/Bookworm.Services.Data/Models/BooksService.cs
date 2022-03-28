namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class BooksService : IBooksService
    {
        private readonly IRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Language> languagesRepository;
        private readonly IRepository<AuthorBook> authorsBooksRepository;
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly IDeletableEntityRepository<Publisher> publishersRepository;
        private readonly IDeletableEntityRepository<Comment> commentRepository;
        private readonly IRepository<Vote> votesRepository;

        public BooksService(
            IRepository<Category> categoriesRepository,
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Language> languagesRepository,
            IRepository<AuthorBook> authorsBooksRepository,
            IDeletableEntityRepository<Author> authorRepository,
            IDeletableEntityRepository<Publisher> publishersRepository,
            IDeletableEntityRepository<Comment> commentRepository,
            IRepository<Vote> votesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.bookRepository = bookRepository;
            this.languagesRepository = languagesRepository;
            this.authorsBooksRepository = authorsBooksRepository;
            this.authorRepository = authorRepository;
            this.publishersRepository = publishersRepository;
            this.commentRepository = commentRepository;
            this.votesRepository = votesRepository;
        }

        public IEnumerable<SelectListItem> GetBookCategories()
        {
            return this.categoriesRepository
                .AllAsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                })
                .ToList();
        }

        public BookViewModel GetBookWithId(string bookId, string userId = null)
        {
            Book book = this.bookRepository.All().First(x => x.Id == bookId);

            string category = this.categoriesRepository
                .AllAsNoTracking()
                .First(c => c.Id == book.CategoryId)
                .Name;

            List<int> authorsIds = this.authorsBooksRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Select(x => x.AuthorId)
                .ToList();

            List<string> authors = this.authorRepository
                .AllAsNoTracking()
                .Where(x => authorsIds.Contains(x.Id))
                .Select(x => x.Name)
                .ToList();

            string publisher = this.publishersRepository.AllAsNoTracking().First(x => x.Id == book.PublisherId).Name;

            string language = this.languagesRepository
                .AllAsNoTracking()
                .First(l => l.Id == book.LanguageId)
                .Name;

            double votesAvg = 0;
            int votesCount = this.votesRepository.AllAsNoTracking().Where(x => x.BookId == bookId).Count();

            if (votesCount > 0)
            {
                votesAvg = this.votesRepository.All().Where(x => x.BookId == bookId).Average(x => x.Value);
            }

            List<CommentViewModel> comments = this.commentRepository
                .All()
                .Where(x => x.BookId == bookId)
                .To<CommentViewModel>()
                .ToList();

            return this.bookRepository
              .AllAsNoTracking()
              .Where(x => x.Id == bookId)
              .Select(x => new BookViewModel()
              {
                  Id = x.Id,
                  Title = x.Title,
                  Description = x.Description,
                  DownloadsCount = x.DownloadsCount,
                  FileUrl = x.FileUrl,
                  ImageUrl = x.ImageUrl,
                  Language = language,
                  PagesCount = x.PagesCount,
                  Year = x.Year,
                  Authors = authors,
                  Publisher = publisher,
                  CategoryName = category,
                  VotesAvg = votesAvg,
                  VotesCount = votesCount,
                  UserVote = this.votesRepository.All().FirstOrDefault(x => x.BookId == bookId && x.UserId == userId).Value,
                  Comments = comments,
              }).FirstOrDefault();
        }

        public BookListingViewModel GetBooks(int categoryId, int page, int booksPerPage)
        {
            return new BookListingViewModel()
            {
                CategoryName = this.categoriesRepository.AllAsNoTracking().First(x => x.Id == categoryId).Name,
                Books = this.bookRepository
                            .AllAsNoTracking()
                            .Where(x => x.CategoryId == categoryId)
                            .To<BookViewModel>()
                            .Skip((page - 1) * booksPerPage)
                            .Take(booksPerPage)
                            .OrderByDescending(x => x.Id)
                            .ToList(),
                PageNumber = page,
                BookCount = this.bookRepository.AllAsNoTracking().Where(x => x.CategoryId == categoryId).Count(),
                BooksPerPage = booksPerPage,
            };
        }
    }
}
