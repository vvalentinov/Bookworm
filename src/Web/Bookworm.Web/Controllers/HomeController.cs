namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IRetrieveBooksService retrieveBooksService;
        private readonly IRetrieveQuotesService retrieveQuotesService;

        public HomeController(
            IRetrieveBooksService booksService,
            IRetrieveQuotesService retrieveQuotesService)
        {
            this.retrieveBooksService = booksService;
            this.retrieveQuotesService = retrieveQuotesService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel
            {
                RandomQuote = (await this.retrieveQuotesService.GetRandomAsync()).Data,
                RecentBooks = (await this.retrieveBooksService.GetRecentBooksAsync()).Data,
                PopularBooks = (await this.retrieveBooksService.GetPopularBooksAsync()).Data,
            };

            return this.View(model);
        }

        public IActionResult Privacy() => this.View();
    }
}
