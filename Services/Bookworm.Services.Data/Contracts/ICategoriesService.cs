namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICategoriesService
    {
        Task<List<T>> GetAllAsync<T>();

        int GetCategoryId(string categoryName);

        IEnumerable<SelectListItem> GetCategoriesAsSelectListItems();

        Task<string> GetCategoryNameAsync(int categoryId);
    }
}
