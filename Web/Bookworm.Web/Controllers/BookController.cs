namespace Bookworm.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class BookController : Controller
    {
        private readonly IBooksService booksService;
        private readonly IUploadBookService uploadBookService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILanguagesService languagesService;

        public BookController(
            IBooksService booksService,
            IUploadBookService uploadBookService,
            UserManager<ApplicationUser> userManager,
            ILanguagesService languagesService)
        {
            this.booksService = booksService;
            this.uploadBookService = uploadBookService;
            this.userManager = userManager;
            this.languagesService = languagesService;
        }

        [Authorize]
        public IActionResult Upload()
        {
            return this.View(new UploadBookFormModel()
            {
                Categories = this.booksService.GetBookCategories<CategoryViewModel>(),
                Languages = this.languagesService.GetAllLanguages<LanguageViewModel>(),
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(UploadBookFormModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            try
            {
                await this.uploadBookService.UploadBookAsync(
                    model.Title,
                    model.Description,
                    model.LanguageId,
                    model.Publisher,
                    model.PagesCount,
                    model.PublishedYear,
                    model.BookFile,
                    model.ImageFile,
                    model.CategoryId,
                    model.AuthorsNames.Select(x => x.Name),
                    user.Id);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                model.Categories = this.booksService.GetBookCategories<CategoryViewModel>();
                model.Languages = this.languagesService.GetAllLanguages<LanguageViewModel>();
                return this.View(model);
            }

            this.TempData[MessageConstant.SuccessMessage] = "Successfully added book!";
            return this.RedirectToAction("Index", "Home");
        }
    }
}
