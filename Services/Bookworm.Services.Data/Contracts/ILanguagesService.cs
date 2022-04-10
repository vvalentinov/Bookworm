namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ILanguagesService
    {
        IEnumerable<SelectListItem> GetAllLanguages();

        string GetLanguageName(int languageId);
    }
}
