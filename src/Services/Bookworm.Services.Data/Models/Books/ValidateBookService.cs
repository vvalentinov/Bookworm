namespace Bookworm.Services.Data.Models.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CategoryErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.LanguageErrorMessagesConstants;

    public class ValidateBookService : IValidateBookService
    {
        private readonly ILanguagesService languagesService;
        private readonly ICategoriesService categoriesService;
        private readonly ISearchBooksService searchBooksService;

        public ValidateBookService(
            ILanguagesService languagesService,
            ICategoriesService categoriesService,
            ISearchBooksService searchBooksService)
        {
            this.languagesService = languagesService;
            this.categoriesService = categoriesService;
            this.searchBooksService = searchBooksService;
        }

        public async Task<OperationResult> ValidateAsync(
            string title,
            int languageId,
            int categoryId,
            int? bookId = null)
        {
            var bookWithTitleExists = (await this.searchBooksService
                .CheckIfBookWithTitleExistsAsync(title, bookId)).Data;

            if (!bookWithTitleExists)
            {
                return OperationResult.Fail(BookWithTitleExistsError);
            }

            var categoryIdIsValid = (await this.categoriesService
                .CheckIfIdIsValidAsync(categoryId)).Data;

            if (!categoryIdIsValid)
            {
                return OperationResult.Fail(CategoryNotFoundError);
            }

            var languageIdIsValid = (await this.languagesService
                .CheckIfIdIsValidAsync(languageId)).Data;

            if (!languageIdIsValid)
            {
                return OperationResult.Fail(LanguageNotFoundError);
            }

            return OperationResult.Ok();
        }
    }
}
