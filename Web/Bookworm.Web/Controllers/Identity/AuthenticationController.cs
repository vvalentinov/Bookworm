namespace Bookworm.Web.Controllers.Identity
{
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Bookworm.Common.Constants;
    using Bookworm.Data.Models;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.IdentityErrorMessagesConstants;
    using static Bookworm.Common.Constants.GlobalConstants;

    public class AuthenticationController : BaseIdentityController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<LoginViewModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public AuthenticationController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginViewModel> logger,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var externalLogins = (await this.signInManager
                .GetExternalAuthenticationSchemesAsync())
                .ToList();

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = externalLogins,
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

                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");
                    return this.View("Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, LoginError);
                    return this.View(model);
                }
            }

            this.TempData[TempDataMessageConstant.ErrorMessage] = LoginError;
            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var externalLogins = (await this.signInManager
                .GetExternalAuthenticationSchemesAsync())
                .ToList();

            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = externalLogins,
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

            if (await this.userManager.FindByEmailAsync(model.Email) != null)
            {
                this.ModelState.AddModelError("Email", UserWithEmailError);
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
                await this.userManager.AddToRoleAsync(user, UserRoleName);

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = this.Url.Action(
                        "ConfirmEmail",
                        AccountAreaName,
                        new { userId = user.Id, code },
                        this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        this.GetFromEmail(),
                        SystemName,
                        model.Email,
                        user.UserName,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                        this.GetAppPassword());

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

            this.TempData["ErrorMessage"] = RegisterError;
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await this.signInManager.SignOutAsync();
            this.logger.LogInformation("User logged out.");

            return returnUrl != null ?
                this.LocalRedirect(returnUrl) :
                this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Action(nameof(this.ExternalLoginCallback), new { returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(
            string returnUrl = null,
            string remoteError = null)
        {
            returnUrl ??= this.Url.Content("~/");

            var info = await this.signInManager.GetExternalLoginInfoAsync();

            if (remoteError != null || info == null)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = $"Error from external provider: {remoteError}";
                return this.RedirectToAction(nameof(this.Login), new { returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this.signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: true,
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                this.logger.LogInformation($"{info.Principal.Identity.Name} logged in with {info.LoginProvider} provider.");
                return this.LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return this.View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                var model = new ExternalLoginInputModel
                {
                    ReturnUrl = returnUrl,
                    ProviderDisplayName = info.ProviderDisplayName,
                };

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    model.Email = email;
                    var user = await this.userManager.FindByEmailAsync(email);
                    model.UserName = user != null ? user.UserName : string.Empty;
                }

                return this.View(nameof(this.ExternalLogin), model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(
            ExternalLoginInputModel model,
            string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            if (this.ModelState.IsValid == false)
            {
                return this.View(nameof(this.ExternalLogin), model);
            }

            // Get the information about the user from the external login provider
            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = "Error loading external login information during confirmation.";
                return this.RedirectToAction(nameof(this.Login), new { returnUrl });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser { Email = email, UserName = model.UserName };
                var createUserResult = await this.userManager.CreateAsync(user);
                if (createUserResult.Succeeded)
                {
                    this.logger.LogInformation($"User created an account using {info.LoginProvider} provider.");
                }
                else
                {
                    this.TempData[TempDataMessageConstant.ErrorMessage] = "Error while creating the new user! Try again!";
                    return this.View(nameof(this.ExternalLogin), model);
                }
            }

            var result = await this.userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                var userId = await this.userManager.GetUserIdAsync(user);

                if (user.EmailConfirmed == false)
                {
                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = this.Url.Action(
                        "ConfirmEmail",
                        AccountAreaName,
                        new { userId, code },
                        this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        this.GetFromEmail(),
                        SystemName,
                        model.Email,
                        user.UserName,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                        this.GetAppPassword());
                }

                await this.signInManager.SignInAsync(
                    user,
                    isPersistent: false,
                    info.LoginProvider);

                return this.LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            model.ReturnUrl = returnUrl;
            model.ProviderDisplayName = info.ProviderDisplayName;

            return this.View(nameof(this.ExternalLogin), model);
        }

        private string GetFromEmail() => this.configuration.GetValue<string>("MailKitEmailSender:Email");

        private string GetAppPassword() => this.configuration.GetValue<string>("MailKitEmailSender:AppPassword");
    }
}
