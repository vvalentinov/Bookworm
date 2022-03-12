namespace Bookworm.Web.Controllers
{
    using System.Diagnostics;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels;
    using Bookworm.Web.ViewModels.Home;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IQuotesService quotesService;

        public HomeController(IQuotesService quotesService)
        {
            this.quotesService = quotesService;
        }

        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                RandomQuote = this.quotesService.GetRandomQuote<QuoteViewModel>(),
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
