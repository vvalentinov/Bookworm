namespace Bookworm.Web.Controllers.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.AspNetCore.Mvc;

    public class ApiBookController : ApiBaseController
    {
        private readonly ILanguagesService languagesService;
        private readonly ICategoriesService categoriesService;
        private readonly ISearchBooksService searchBooksService;
        private readonly IFavoriteBookService favoriteBookService;

        public ApiBookController(
            ILanguagesService languagesService,
            ICategoriesService categoriesService,
            ISearchBooksService searchBooksService,
            IFavoriteBookService favoriteBookService)
        {
            this.languagesService = languagesService;
            this.categoriesService = categoriesService;
            this.searchBooksService = searchBooksService;
            this.favoriteBookService = favoriteBookService;
        }

        [HttpPost(nameof(SearchBooks))]
        public async Task<ActionResult<BookListingViewModel>> SearchBooks(SearchBookInputModel model)
        {
            if (model.IsForUserBooks)
            {
                model.UserId = this.User.GetId();
            }
            else
            {
                var getCategoryIdResult = await this.categoriesService
                    .GetCategoryIdAsync(model.Category);

                if (getCategoryIdResult.IsFailure)
                {
                    return this.BadRequest(getCategoryIdResult.ErrorMessage);
                }

                model.CategoryId = getCategoryIdResult.Data;
            }

            model.Input ??= string.Empty;

            var result = await this.searchBooksService.SearchBooksAsync(model);

            return this.Ok(result.Data);
        }

        [HttpGet(nameof(GetLanguagesInBookCategory))]
        public async Task<ActionResult<IEnumerable<LanguageViewModel>>> GetLanguagesInBookCategory(string category)
        {
            var getCategoryIdResult = await this.categoriesService
                .GetCategoryIdAsync(category);

            if (getCategoryIdResult.IsFailure)
            {
                return this.BadRequest(getCategoryIdResult.ErrorMessage);
            }

            var getLanguagesResult = await this.languagesService
                .GetAllInBookCategoryAsync(getCategoryIdResult.Data);

            return this.Ok(getLanguagesResult.Data);
        }

        [HttpGet(nameof(GetLanguagesInUserBooks))]
        public async Task<ActionResult<IEnumerable<LanguageViewModel>>> GetLanguagesInUserBooks()
        {
            var userId = this.User.GetId();

            var result = await this.languagesService.GetAllInUserBooksAsync(userId);

            return this.Ok(result.Data);
        }

        [HttpPost(nameof(AddToFavorites))]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var userId = this.User.GetId();

            var result = await this.favoriteBookService
                .AddBookToFavoritesAsync(id, userId);

            if (result.IsSuccess)
            {
                return new JsonResult(result.SuccessMessage);
            }

            return this.BadRequest(new { error = result.ErrorMessage });
        }
    }
}
