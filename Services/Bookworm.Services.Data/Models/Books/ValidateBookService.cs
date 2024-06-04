namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Threading.Tasks;

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

        public async Task ValidateAsync(
            string title,
            int languageId,
            int categoryId,
            int? bookId = null)
        {
            if (await this.searchBooksService.CheckIfBookWithTitleExistsAsync(title, bookId))
            {
                throw new InvalidOperationException(BookWithTitleExistsError);
            }

            if (!await this.categoriesService.CheckIfIdIsValidAsync(categoryId))
            {
                throw new InvalidOperationException(CategoryNotFoundError);
            }

            if (!await this.languagesService.CheckIfIdIsValidAsync(languageId))
            {
                throw new InvalidOperationException(LanguageNotFoundError);
            }
        }
    }
}
