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
        public async Task<BookListingViewModel> SearchBooks(string input, int page, string category)
        {
            try
            {
                var categoryId = await this.categoriesService.GetCategoryIdAsync(category);

                var books = await this.searchBooksService.SearchBooks(input ?? string.Empty, page, categoryId);
                books.PaginationController = "ApiBook";
                books.PaginationAction = nameof(this.SearchBooks);

                return books;
            }
            catch
            {
                return new BookListingViewModel();
            }
        }

        [HttpGet(nameof(SearchUserBooks))]
        public async Task<BookListingViewModel> SearchUserBooks(string input, int page)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);

                var books = await this.searchBooksService.SearchUserBooks(input ?? string.Empty, page, userId);
                books.PaginationController = "ApiBook";
                books.PaginationAction = nameof(this.SearchUserBooks);

                return books;
            }
            catch
            {
                return new BookListingViewModel();
            }
        }
    }
}
