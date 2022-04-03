namespace Bookworm.Web.Controllers
{
    using System.Diagnostics;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Home;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IQuotesService quotesService;
        private readonly IBooksService booksService;

        public HomeController(IQuotesService quotesService, IBooksService booksService)
        {
            this.quotesService = quotesService;
            this.booksService = booksService;
        }

        public IActionResult Index()
        {
            IndexViewModel model = new()
            {
                RandomQuote = this.quotesService.GetRandomQuote<QuoteViewModel>(),
                RecentBooks = this.booksService.GetRecentBooks<BookViewModel>(12),
                PopularBooks = this.booksService.GetPopularBooks<BookViewModel>(12),
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
