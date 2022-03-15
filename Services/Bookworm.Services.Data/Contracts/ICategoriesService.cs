namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    public interface ICategoriesService
    {
        IList<T> GetAll<T>();

        int GetCategoryId(string categoryName);
    }
}
