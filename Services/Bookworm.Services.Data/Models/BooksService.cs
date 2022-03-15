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

        public BooksService(IRepository<Category> categoriesRepository, IDeletableEntityRepository<Book> bookRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.bookRepository = bookRepository;
        }

        public IEnumerable<T> GetBookCategories<T>()
        {
            return this.categoriesRepository
                .AllAsNoTracking()
                .OrderBy(x => x.Name)
                .To<T>()
                .ToList();
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
