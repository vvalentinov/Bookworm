namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Web.StaticKeys.ViewDataKeys;

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
            var getUnapprovedBooksCountResult = await this.booksService
                .GetUnapprovedBooksCountAsync();

            var getUnapprovedQuotesCountResult = await this.retrieveQuotesService
                .GetUnapprovedCountAsync();

            this.ViewData[BooksCount] = getUnapprovedBooksCountResult.Data;
            this.ViewData[QuotesCount] = getUnapprovedQuotesCountResult.Data;

            return this.View();
        }
    }
}
