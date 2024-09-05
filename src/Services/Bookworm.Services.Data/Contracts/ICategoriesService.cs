namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.Categories;

    public interface ICategoriesService
    {
        Task<OperationResult<IEnumerable<CategoryViewModel>>> GetAllAsync();

        Task<OperationResult<int>> GetCategoryIdAsync(string categoryName);

        Task<OperationResult<bool>> CheckIfIdIsValidAsync(int categoryId);
    }
}
