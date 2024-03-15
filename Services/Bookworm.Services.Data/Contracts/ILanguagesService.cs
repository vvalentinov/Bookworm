namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Languages;

    public interface ILanguagesService
    {
        Task<List<LanguageViewModel>> GetAllAsync();

        Task<string> GetLanguageNameAsync(int languageId);

        Task<List<LanguageViewModel>> GetAllInUserBooksAsync(string userId);

        Task<List<LanguageViewModel>> GetAllInBookCategoryAsync(int categoryId);

        Task<bool> CheckIfIdIsValidAsync(int languageId);
    }
}
