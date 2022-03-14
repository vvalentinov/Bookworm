namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    public interface ILanguagesService
    {
        IEnumerable<T> GetAllLanguages<T>();
    }
}
