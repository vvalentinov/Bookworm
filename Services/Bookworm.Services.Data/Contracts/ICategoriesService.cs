namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        Task<List<T>> GetAllAsync<T>();

        Task<int> GetCategoryIdAsync(string categoryName);

        Task<bool> CheckIfIdIsValid(int categoryId);
    }
}
