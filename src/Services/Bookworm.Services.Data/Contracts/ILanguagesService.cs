namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.Languages;

    public interface ILanguagesService
    {
        Task<OperationResult<bool>> CheckIfIdIsValidAsync(int languageId);

        Task<OperationResult<IEnumerable<LanguageViewModel>>> GetAllAsync();

        Task<OperationResult<IEnumerable<LanguageViewModel>>> GetAllInUserBooksAsync(string userId);

        Task<OperationResult<IEnumerable<LanguageViewModel>>> GetAllInBookCategoryAsync(int categoryId);
    }
}
