namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiBookController : BaseApiController
    {
        private readonly ISearchBooksService searchBooksService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiBookController(
            ISearchBooksService searchBooksService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager)
        {
            this.searchBooksService = searchBooksService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
        }

        [HttpGet(nameof(SearchBooks))]
        public async Task<BookListingViewModel> SearchBooks([FromQuery]SearchBookInputModel model)
        {
            try
            {
                model.CategoryId = await this.categoriesService.GetCategoryIdAsync(model.Category);

                if (model.IsForUserBooks)
                {
                    model.UserId = this.userManager.GetUserId(this.User);
                }

                model.Input ??= string.Empty;

                var books = await this.searchBooksService.SearchBooks(model);
                books.PaginationController = "ApiBook";
                books.PaginationAction = nameof(this.SearchBooks);

                return books;
            }
            catch
            {
                return new BookListingViewModel();
            }
        }
    }
}
