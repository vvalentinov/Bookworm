namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IRetrieveBooksService booksService;
        private readonly IRetrieveQuotesService retrieveQuotesService;

        public HomeController(
            IRetrieveBooksService booksService,
            IRetrieveQuotesService retrieveQuotesService)
        {
            this.booksService = booksService;
            this.retrieveQuotesService = retrieveQuotesService;
        }

        public async Task<IActionResult> Index()
        {
            int unapprovedBooksCount = await this.booksService.GetUnapprovedBooksCountAsync();
            int unapprovedQuotesCount = await this.retrieveQuotesService.GetUnapprovedCountAsync();

            this.ViewData["BooksCount"] = unapprovedBooksCount;
            this.ViewData["QuotesCount"] = unapprovedQuotesCount;

            return this.View();
        }
    }
}
