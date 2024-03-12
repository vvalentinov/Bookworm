namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Languages;

    public interface ILanguagesService
    {
        Task<List<LanguageViewModel>> GetAllAsync();

        Task<string> GetLanguageNameAsync(int languageId);

        Task<List<LanguageViewModel>> GetAllInBookCategory(int categoryId);

        Task<List<LanguageViewModel>> GetAllInUserBooksAsync(string userId);
    }
}
