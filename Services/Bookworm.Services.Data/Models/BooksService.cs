namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Books;

    public class BooksService : IBooksService
    {
        private readonly IRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Language> languagesRepository;

        public BooksService(
            IRepository<Category> categoriesRepository,
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Language> languagesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.bookRepository = bookRepository;
            this.languagesRepository = languagesRepository;
        }

        public IEnumerable<T> GetBookCategories<T>()
        {
            return this.categoriesRepository
                .AllAsNoTracking()
                .OrderBy(x => x.Name)
                .To<T>()
                .ToList();
        }

        public BookViewModel GetBookWithId(string id)
        {
            Book book = this.bookRepository.AllAsNoTracking().First(x => x.Id == id);
            string language = this.languagesRepository.AllAsNoTracking().First(l => l.Id == book.LanguageId).Name;
            return this.bookRepository
              .AllAsNoTracking()
              .Where(x => x.Id == id)
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
