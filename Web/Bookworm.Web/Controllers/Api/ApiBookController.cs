namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Mvc;

    public class ApiBookController : BaseApiController
    {
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly ICategoriesService categoriesService;

        public ApiBookController(
            IRetrieveBooksService retrieveBooksService,
            ICategoriesService categoriesService)
        {
            this.retrieveBooksService = retrieveBooksService;
            this.categoriesService = categoriesService;
        }

        [HttpGet(nameof(SearchBook))]
        public async Task<BookListingViewModel> SearchBook(
            string input,
            int page,
            string category)
        {
            try
            {
                var categoryId = await this.categoriesService.GetCategoryIdAsync(category);
                var model = await this.retrieveBooksService.SearchBooks(input ?? string.Empty, page, categoryId);

                return model;
            }
            catch
            {
                return new BookListingViewModel();
            }
        }
    }
}
