namespace Bookworm.Web.Areas.Identity.Controllers
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Bookworm.Common.Constants;
    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;

    public class UserController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<LoginViewModel> logger;

        public UserController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginViewModel> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
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

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model,
            string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            // this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (await this.userManager.FindByEmailAsync(model.Email) != null)
            {
                this.ModelState.AddModelError("Email", "User with given email already exist!");
                return this.View(model);
            }

            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };

                var result = await this.userManager.CreateAsync(user, model.Password);
                await this.userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code, returnUrl },
                        protocol: this.Request.Scheme);

                    // await this.emailSender.SendEmailAsync(this.Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return this.RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl });
                    }
                    else
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            this.TempData["ErrorMessage"] = "Register failed! Try again!";
            return this.View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return this.View();
        }
    }
}
