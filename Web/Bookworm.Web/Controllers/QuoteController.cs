﻿namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class QuoteController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuotesService quotesService;

        public QuoteController(UserManager<ApplicationUser> userManager, IQuotesService quotesService)
        {
            this.userManager = userManager;
            this.quotesService = quotesService;
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            QuoteViewModel quote = this.quotesService.GetQuoteById(id);
            return this.View(quote);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(QuoteViewModel quote)
        {
            await this.quotesService.EditQuoteAsync(
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

            UserQuotesViewModel quotes = await this.quotesService.GetUserQuotesAsync(user.Id);

            return this.View(quotes);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await this.quotesService.DeleteQuoteAsync(id);
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

            QuoteListingViewModel quotes = await this.quotesService.GetAllQuotesAsync(userId);
            return this.View(quotes);
        }

        [Authorize]
        public IActionResult Add()
        {
            UploadQuoteViewModel model = new UploadQuoteViewModel()
            {
                MovieQuoteImgUrl = this.quotesService.GetMovieQuoteImageUrl(),
                BookQuoteImgUrl = this.quotesService.GetBookQuoteImageUrl(),
                GeneralQuoteImgUrl = this.quotesService.GetGeneralQuoteImageUrl(),
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

            if (await this.quotesService.QuoteExists(generalQuoteModel.Content))
            {
                this.TempData[MessageConstant.ErrorMessage] = "This quote already exist! Try again!";
                return this.View("Add");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.quotesService.AddGeneralQuoteAsync(
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

            if (await this.quotesService.QuoteExists(movieQuoteModel.Content))
            {
                this.TempData[MessageConstant.WarningMessage] = "This quote already exist! Try again!";
                return this.View("Add");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.quotesService.AddMovieQuoteAsync(
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

            if (await this.quotesService.QuoteExists(bookQuoteModel.Content))
            {
                this.TempData[MessageConstant.WarningMessage] = "This quote already exist! Try again!";
                return this.View("Add");
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            await this.quotesService.AddBookQuoteAsync(
                bookQuoteModel.Content,
                bookQuoteModel.BookTitle,
                bookQuoteModel.AuthorName,
                user.Id);

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added quote!";

            return this.RedirectToAction("Index", "Home");
        }
    }
}
