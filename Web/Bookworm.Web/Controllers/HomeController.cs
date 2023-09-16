namespace Bookworm.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels;
    using Bookworm.Web.ViewModels.Home;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IRetrieveBooksService booksService;

        public HomeController(
            IRetrieveQuotesService retrieveQuotesService,
            IRetrieveBooksService booksService)
        {
            this.retrieveQuotesService = retrieveQuotesService;
            this.booksService = booksService;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel model = new ()
            {
                RandomQuote = await this.retrieveQuotesService.GetRandomQuoteAsync<QuoteViewModel>(),
                RecentBooks = await this.booksService.GetRecentBooksAsync(12),
                PopularBooks = await this.booksService.GetPopularBooksAsync(12),
            };

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
