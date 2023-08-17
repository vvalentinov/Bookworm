namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class QuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IUpdateQuoteService updateQuoteService;
        private readonly IUploadQuoteService uploadQuoteService;
        private readonly IGetQuoteTypeImgUrlService getQuoteTypeImgUrlService;
        private readonly ICheckIfQuoteExistsService checkIfQuoteExistsService;

        public QuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IUpdateQuoteService updateQuoteService,
            IUploadQuoteService uploadQuoteService,
            IGetQuoteTypeImgUrlService getQuoteTypeImgUrlService,
            ICheckIfQuoteExistsService checkIfQuoteExistsService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.updateQuoteService = updateQuoteService;
            this.uploadQuoteService = uploadQuoteService;
            this.getQuoteTypeImgUrlService = getQuoteTypeImgUrlService;
            this.checkIfQuoteExistsService = checkIfQuoteExistsService;
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            QuoteViewModel quote = this.retrieveQuotesService.GetQuoteById(id);
            return this.View(quote);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(QuoteViewModel quote)
        {
            await this.updateQuoteService.EditQuoteAsync(
                quote.Id,
                quote.Content,
                quote.AuthorName,
                quote.BookTitle,
                quote.MovieTitle);

            this.TempData[MessageConstant.SuccessMessage] = "Successfully edited quote";

            return this.RedirectToAction(nameof(this.UserQuotes), "Quote");
        }

        [Authorize]
        public async Task<IActionResult> UserQuotes()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            UserQuotesViewModel quotes = await this.retrieveQuotesService.GetUserQuotesAsync(user.Id);

            return this.View(quotes);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await this.updateQuoteService.DeleteQuoteAsync(id);
            this.TempData[MessageConstant.SuccessMessage] = "Successfully deleted quote!";
            return this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> All()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            string userId = null;
            if (user != null)
            {
                userId = user.Id;
            }

            QuoteListingViewModel quotes = await this.retrieveQuotesService.GetAllQuotesAsync(userId);
            return this.View(quotes);
        }

        [Authorize]
        public IActionResult Add()
        {
            UploadQuoteViewModel model = new UploadQuoteViewModel()
            {
                MovieQuoteImgUrl = this.getQuoteTypeImgUrlService.GetMovieQuoteImageUrl(),
                BookQuoteImgUrl = this.getQuoteTypeImgUrlService.GetBookQuoteImageUrl(),
                GeneralQuoteImgUrl = this.getQuoteTypeImgUrlService.GetGeneralQuoteImageUrl(),
            };

            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(UploadGeneralQuoteViewModel generalQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View("Add");
            }

            if (await this.checkIfQuoteExistsService.QuoteExists(generalQuoteModel.Content))
            {
                this.TempData[MessageConstant.ErrorMessage] = "This quote already exist! Try again!";
                return this.View("Add");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.uploadQuoteService.UploadGeneralQuoteAsync(
                generalQuoteModel.Content,
                generalQuoteModel.AuthorName,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added quote!";

            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddMovieQuote(UploadMovieQuoteViewModel movieQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View("Add");
            }

            if (await this.checkIfQuoteExistsService.QuoteExists(movieQuoteModel.Content))
            {
                this.TempData[MessageConstant.WarningMessage] = "This quote already exist! Try again!";
                return this.View("Add");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.uploadQuoteService.UploadMovieQuoteAsync(
                movieQuoteModel.Content,
                movieQuoteModel.MovieTitle,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added quote!";

            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBookQuote(UploadBookQuoteViewModel bookQuoteModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View("Add");
            }

            if (await this.checkIfQuoteExistsService.QuoteExists(bookQuoteModel.Content))
            {
                this.TempData[MessageConstant.WarningMessage] = "This quote already exist! Try again!";
                return this.View("Add");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.uploadQuoteService.UploadBookQuoteAsync(
                bookQuoteModel.Content,
                bookQuoteModel.BookTitle,
                bookQuoteModel.AuthorName,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added quote!";

            return this.RedirectToAction("Index", "Home");
        }
    }
}
