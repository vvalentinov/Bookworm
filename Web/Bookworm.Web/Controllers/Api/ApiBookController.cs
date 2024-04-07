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

    public class ApiBookController : BaseApiController
    {
        private readonly ISearchBooksService searchBooksService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILanguagesService languagesService;

        public ApiBookController(
            ISearchBooksService searchBooksService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager,
            ILanguagesService languagesService)
        {
            this.searchBooksService = searchBooksService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
            this.languagesService = languagesService;
        }

        [HttpPost(nameof(SearchBooks))]
        public async Task<ActionResult<BookListingViewModel>> SearchBooks([FromBody]SearchBookInputModel model)
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
        public async Task<ActionResult<List<LanguageViewModel>>> GetLanguagesInBookCategory(string category)
        {
            try
            {
                int categoryId = await this.categoriesService.GetCategoryIdAsync(category);
                return await this.languagesService.GetAllInBookCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet(nameof(GetLanguagesInUserBooks))]
        public async Task<List<LanguageViewModel>> GetLanguagesInUserBooks()
        {
            var userId = this.userManager.GetUserId(this.User);
            return await this.languagesService.GetAllInUserBooksAsync(userId);
        }
    }
}
