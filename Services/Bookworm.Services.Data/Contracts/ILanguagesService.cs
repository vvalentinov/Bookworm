namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Languages;

    public interface ILanguagesService
    {
        IEnumerable<LanguageViewModel> GetAllLanguages();

        string GetLanguageName(int languageId);
    }
}
