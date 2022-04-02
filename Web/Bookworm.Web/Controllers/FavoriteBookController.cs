namespace Bookworm.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class FavoriteBookController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFavoriteBooksService favoriteBooksService;

        public FavoriteBookController(UserManager<ApplicationUser> userManager, IFavoriteBooksService favoriteBooksService)
        {
            this.userManager = userManager;
            this.favoriteBooksService = favoriteBooksService;
        }

        [Authorize]
        public async Task<IActionResult> AddToFavorites(string bookId)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            try
            {
                await this.favoriteBooksService.AddBookToFavoritesAsync(bookId, user.Id);
                this.TempData[MessageConstant.SuccessMessage] = "Successfully added book to favorites!";
            }
            catch (Exception ex)
            {
                this.TempData[MessageConstant.WarningMessage] = ex.Message;
                return this.RedirectToAction("CurrentBook", new { id = bookId });
            }

            return this.RedirectToAction("CurrentBook", new { id = bookId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteFromFavorites(string bookId)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            await this.favoriteBooksService.DeleteFromFavoritesAsync(bookId, user.Id);
            this.TempData[MessageConstant.SuccessMessage] = "Successfully removed book!";
            return this.RedirectToAction("Favorites");
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            var books = this.favoriteBooksService.GetUserFavoriteBooks(user.Id);
            return this.View(books);
        }
    }
}
