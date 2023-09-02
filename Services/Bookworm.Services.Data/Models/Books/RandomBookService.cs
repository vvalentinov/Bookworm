namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;

    public class RandomBookService : IRandomBookService
    {
        private readonly IRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public RandomBookService(
            IRepository<Category> categoriesRepository,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.bookRepository = bookRepository;
        }

        public IEnumerable<BookViewModel> GenerateBooks(string category, int countBooks)
        {
            int categoryId = this.categoriesRepository
                                 .AllAsNoTracking()
                                 .First(x => x.Name == category)
                                 .Id;

            List<BookViewModel> books = this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.CategoryId == categoryId)
                .OrderBy(x => Guid.NewGuid())
                .Take(countBooks)
                .Select(x => new BookViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ImageUrl = x.ImageUrl,
                }).ToList();

            return books;
        }
    }
}
