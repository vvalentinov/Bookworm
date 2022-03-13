namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;

    public class BooksService : IBooksService
    {
        private readonly IRepository<Category> categoriesRepository;

        public BooksService(IRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public IEnumerable<T> GetBookCategories<T>()
        {
            return this.categoriesRepository
                .AllAsNoTracking()
                .OrderBy(x => x.Name)
                .To<T>()
                .ToList();
        }
    }
}
