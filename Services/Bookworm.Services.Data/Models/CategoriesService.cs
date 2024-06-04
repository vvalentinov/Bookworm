namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> categoriesRepository;

        public CategoriesService(IRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<List<T>> GetAllAsync<T>()
            => await this.categoriesRepository
                    .AllAsNoTracking()
                    .OrderBy(x => x.Name)
                    .To<T>()
                    .ToListAsync();

        public async Task<int> GetCategoryIdAsync(string categoryName)
        {
            var category = await this.categoriesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == categoryName.Trim()) ??
                throw new InvalidOperationException("The given category doesn't exist!");

            return category.Id;
        }

        public async Task<bool> CheckIfIdIsValidAsync(int categoryId)
            => await this.categoriesRepository.AllAsNoTracking().AnyAsync(c => c.Id == categoryId);
    }
}
