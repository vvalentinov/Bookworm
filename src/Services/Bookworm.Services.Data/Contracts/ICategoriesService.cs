namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;

    public interface ICategoriesService
    {
        Task<OperationResult<IEnumerable<T>>> GetAllAsync<T>();

        Task<OperationResult<int>> GetCategoryIdAsync(string categoryName);

        Task<OperationResult<bool>> CheckIfIdIsValidAsync(int categoryId);
    }
}
