namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiBookController : ApiBaseController
    {
        private readonly ILanguagesService languagesService;
        private readonly ICategoriesService categoriesService;
        private readonly ISearchBooksService searchBooksService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFavoriteBookService favoriteBookService;

        public ApiBookController(
            ILanguagesService languagesService,
            ICategoriesService categoriesService,
            ISearchBooksService searchBooksService,
            UserManager<ApplicationUser> userManager,
            IFavoriteBookService favoriteBookService)
        {
            this.userManager = userManager;
            this.languagesService = languagesService;
            this.categoriesService = categoriesService;
            this.searchBooksService = searchBooksService;
            this.favoriteBookService = favoriteBookService;
        }

        [HttpPost(nameof(SearchBooks))]
        public async Task<ActionResult<BookListingViewModel>> SearchBooks([FromBody] SearchBookInputModel model)
        {
            try
            {
                if (model.IsForUserBooks)
                {
                    model.UserId = this.userManager.GetUserId(this.User);
                }
                else
                {
                    model.CategoryId = await this.categoriesService.GetCategoryIdAsync(model.Category);
                }

                model.Input ??= string.Empty;

                var books = await this.searchBooksService.SearchBooksAsync(model);

                return books;
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet(nameof(GetLanguagesInBookCategory))]
        public async Task<ActionResult<IEnumerable<LanguageViewModel>>> GetLanguagesInBookCategory(string category)
        {
            try
            {
                int categoryId = await this.categoriesService.GetCategoryIdAsync(category);
                return this.Ok(await this.languagesService.GetAllInBookCategoryAsync(categoryId));
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet(nameof(GetLanguagesInUserBooks))]
        public async Task<ActionResult<IEnumerable<LanguageViewModel>>> GetLanguagesInUserBooks()
        {
            var userId = this.userManager.GetUserId(this.User);
            return this.Ok(await this.languagesService.GetAllInUserBooksAsync(userId));
        }

        [HttpPost(nameof(AddToFavorites))]
        public async Task<IActionResult> AddToFavorites([FromQuery] int id)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);
                await this.favoriteBookService.AddBookToFavoritesAsync(id, userId);
                return new JsonResult("Successfully added to favorites list!");
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { error = ex.Message });
            }
        }
    }
}
