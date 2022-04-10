namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICategoriesService
    {
        IList<T> GetAll<T>();

        int GetCategoryId(string categoryName);

        IEnumerable<SelectListItem> GetCategoriesAsSelectListItems();

        string GetCategoryName(int categoryId);
    }
}
