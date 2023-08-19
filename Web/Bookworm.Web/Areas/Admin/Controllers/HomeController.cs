namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Linq;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IBooksService booksService;
        private readonly IRetrieveQuotesService retrieveQuotesService;

        public HomeController(
            IBooksService booksService,
            IRetrieveQuotesService retrieveQuotesService)
        {
            this.booksService = booksService;
            this.retrieveQuotesService = retrieveQuotesService;
        }

        public IActionResult Index()
        {
            int unapprovedBooksCount = this.booksService.GetUnapprovedBooks().ToList().Count;
            int unapprovedQuotesCount = this.retrieveQuotesService.GetAllUnapprovedQuotes().Quotes.Count;
            this.ViewData["BooksCount"] = unapprovedBooksCount;
            this.ViewData["QuotesCount"] = unapprovedQuotesCount;
            return this.View();
        }
    }
}
