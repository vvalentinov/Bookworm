namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    public interface IBooksService
    {
        IEnumerable<T> GetBookCategories<T>();
    }
}
