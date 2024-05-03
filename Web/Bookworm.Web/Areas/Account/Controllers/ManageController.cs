namespace Bookworm.Web.Areas.Account.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class ManageController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ChangePasswordInputModel> logger;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordInputModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public IActionResult Index() => this.View();

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                var userId = this.userManager.GetUserId(this.User);
                return this.NotFound($"Unable to load user with ID '{userId}'.");
            }

            //var hasPassword = await this.userManager.HasPasswordAsync(user);
            //if (!hasPassword)
            //{
            //    return RedirectToPage("./SetPassword");
            //}

            var model = new ChangePasswordInputModel();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                var userId = this.userManager.GetUserId(this.User);
                return this.NotFound($"Unable to load user with ID '{userId}'.");
            }

            var changePasswordResult = await this.userManager.ChangePasswordAsync(
                user,
                model.OldPassword,
                model.NewPassword);

            if (changePasswordResult.Succeeded == false)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.View(model);
            }

            await this.signInManager.RefreshSignInAsync(user);
            this.logger.LogInformation("User changed their password successfully.");
            this.TempData[SuccessMessage] = "Your password has been changed.";
            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
