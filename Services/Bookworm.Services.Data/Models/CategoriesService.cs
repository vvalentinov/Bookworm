namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> categoriesRepository;

        public CategoriesService(IRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public IList<T> GetAll<T>()
        {
            return this.categoriesRepository
            .AllAsNoTracking()
            .OrderBy(x => x.Name)
            .To<T>()
            .ToList();
        }

        public IEnumerable<SelectListItem> GetCategoriesAsSelectListItems()
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

        public int GetCategoryId(string categoryName)
        {
            return this.categoriesRepository
                .AllAsNoTracking()
                .First(x => x.Name == categoryName)
                .Id;
        }

        public async Task<string> GetCategoryNameAsync(int categoryId)
        {
            Category category = await this.categoriesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            return category?.Name;
        }
    }
}
