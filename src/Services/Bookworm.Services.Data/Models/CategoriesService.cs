namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;

    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> categoriesRepository;

        public CategoriesService(IRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<OperationResult<IEnumerable<T>>> GetAllAsync<T>()
        {
            var data = await this.categoriesRepository
                    .AllAsNoTracking()
                    .OrderBy(x => x.Name)
                    .To<T>()
                    .ToListAsync();

            return OperationResult.Ok<IEnumerable<T>>(data);
        }

        public async Task<OperationResult<int>> GetCategoryIdAsync(string categoryName)
        {
            var category = await this.categoriesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == categoryName.Trim());

            if (category == null)
            {
                return OperationResult.Fail<int>(CategoryNotFoundError);
            }

            return OperationResult.Ok<int>(category.Id);
        }

        public async Task<OperationResult<bool>> CheckIfIdIsValidAsync(int categoryId)
        {
            var isValid = await this.categoriesRepository
                .AllAsNoTracking()
                .AnyAsync(c => c.Id == categoryId);

            return OperationResult.Ok<bool>(isValid);
        }
    }
}
