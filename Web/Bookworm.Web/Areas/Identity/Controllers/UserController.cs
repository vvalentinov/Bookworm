namespace Bookworm.Web.Areas.Identity.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class UserController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LoginViewModel> logger;

        public UserController(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginViewModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this.signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");
                    return this.LocalRedirect(returnUrl);
                }

                // if (result.RequiresTwoFactor)
                // {
                //    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, this.Input.RememberMe });
                // }

                // if (result.IsLockedOut)
                // {
                //    this.logger.LogWarning("User account locked out.");
                //    return this.RedirectToPage("./Lockout");
                // }
                // else
                // {
                //    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return this.Page();
                // }
            }

            this.TempData["ErrorMessage"] = "Login failed! Try again!";
            return this.View(model);
        }
    }
}
