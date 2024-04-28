namespace Bookworm.Web.Controllers
{
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Bookworm.Common.Constants;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class UserController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<LoginViewModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public UserController(
            IUsersService usersService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginViewModel> logger,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.usersService = usersService;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");
                    return this.LocalRedirect(returnUrl);
                }

                // if (result.RequiresTwoFactor)
                // {
                //    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, this.Input.RememberMe });
                // }
                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");
                    return this.View("Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.View(model);
                }
            }

            this.TempData["ErrorMessage"] = "Login failed! Try again!";
            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await this.signInManager.SignOutAsync();
            this.logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return this.LocalRedirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Statistics()
        {
            var users = this.usersService.GetUsersStatistics();
            return this.View(users);
        }

        [HttpGet]
        public IActionResult AccessDenied() => this.View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => this.View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(EmailInputViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null || !await this.userManager.IsEmailConfirmedAsync(user))
            {
                return this.RedirectToAction("Index", "Home", new { area = string.Empty });
            }

            var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = this.Url.Action(
                "ResetPassword",
                "User",
                new { code },
                this.Request.Scheme);

            var fromEmail = this.configuration.GetValue<string>("MailKitEmailSender:Email");
            var appPassword = this.configuration.GetValue<string>("MailKitEmailSender:AppPassword");

            await this.emailSender.SendEmailAsync(
                fromEmail,
                "Bookworm",
                model.Email,
                user.UserName,
                "Password reset",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                appPassword);

            this.TempData[TempDataMessageConstant.SuccessMessage] = "Please check your email to reset your password.";
            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return this.BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                var model = new ResetPasswordInputModel { Code = code };
                return this.View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordInputModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View();
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = "There was a problem resetting your password! Please, try again!";
                return this.RedirectToAction("Index", "Home");
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

            var result = await this.userManager.ResetPasswordAsync(
                user,
                code,
                model.Password);
            if (result.Succeeded)
            {
                return this.View("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation() => this.View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation(EmailInputViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return this.View(model);
                }

                var userId = await this.userManager.GetUserIdAsync(user);

                var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = this.Url.Action("ConfirmEmail", "User", new { userId, code }, this.Request.Scheme);

                var fromEmail = this.configuration.GetValue<string>("MailKitEmailSender:Email");
                var appPassword = this.configuration.GetValue<string>("MailKitEmailSender:AppPassword");

                await this.emailSender.SendEmailAsync(
                    fromEmail,
                    "Bookworm",
                    model.Email,
                    user.UserName,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                    appPassword);

                this.TempData[TempDataMessageConstant.SuccessMessage] = "Verification email sent. Please check your email.";
                return this.RedirectToAction("Index", "Home", new { area = string.Empty });
            }

            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null || code == null)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = "There was a problem confirming your email! Try again!";
                return this.RedirectToAction("Index", "Home", new { area = string.Empty });
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await this.userManager.ConfirmEmailAsync(user, code);

            this.ViewData["Message"] = result.Succeeded ?
                "Thank you for confirming your email." :
                "Error confirming your email.";
            return this.View();
        }
    }
}
