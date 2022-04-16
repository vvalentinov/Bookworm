namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Linq;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IBooksService booksService;
        private readonly IQuotesService quotesService;

        public HomeController(IBooksService booksService, IQuotesService quotesService)
        {
            this.booksService = booksService;
            this.quotesService = quotesService;
        }

        public IActionResult Index()
        {
            int unapprovedBooksCount = this.booksService.GetUnapprovedBooks().ToList().Count();
            int unapprovedQuotesCount = this.quotesService.GetAllUnapprovedQuotes<QuoteViewModel>().ToList().Count();
            this.ViewData["BooksCount"] = unapprovedBooksCount;
            this.ViewData["QuotesCount"] = unapprovedQuotesCount;
            return this.View();
        }
    }
}
