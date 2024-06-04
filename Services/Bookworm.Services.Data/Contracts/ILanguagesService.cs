namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Languages;

    public interface ILanguagesService
    {
        Task<bool> CheckIfIdIsValidAsync(int languageId);

        Task<IEnumerable<LanguageViewModel>> GetAllAsync();

        Task<IEnumerable<LanguageViewModel>> GetAllInUserBooksAsync(string userId);

        Task<IEnumerable<LanguageViewModel>> GetAllInBookCategoryAsync(int categoryId);
    }
}
