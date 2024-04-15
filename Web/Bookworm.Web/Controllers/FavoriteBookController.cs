namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Bookworm.Common.Constants;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class FavoriteBookController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFavoriteBooksService favoriteBooksService;

        public FavoriteBookController(
            UserManager<ApplicationUser> userManager,
            IFavoriteBooksService favoriteBooksService)
        {
            this.userManager = userManager;
            this.favoriteBooksService = favoriteBooksService;
        }

        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            var books = this.favoriteBooksService.GetUserFavoriteBooks(user.Id);
            return this.View(books);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            try
            {
                await this.favoriteBooksService.AddBookToFavoritesAsync(id, user.Id);
            }
            catch (Exception)
            {
            }

            return this.Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromFavorites(int bookId)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            await this.favoriteBooksService.DeleteFromFavoritesAsync(bookId, user.Id);
            this.TempData[TempDataMessageConstant.SuccessMessage] = "Successfully removed book!";
            return this.RedirectToAction("Favorites");
        }
    }
}
