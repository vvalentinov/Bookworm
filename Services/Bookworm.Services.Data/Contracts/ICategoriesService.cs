namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        Task<IEnumerable<T>> GetAllAsync<T>();

        Task<int> GetCategoryIdAsync(string categoryName);

        Task<bool> CheckIfIdIsValidAsync(int categoryId);
    }
}
