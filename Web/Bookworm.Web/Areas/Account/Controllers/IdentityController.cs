namespace Bookworm.Web.Areas.Account.Controllers
{
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Bookworm.Common.Constants;
    using Bookworm.Data.Models;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Identity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.IdentityErrorMessagesConstants;

    [Route("[area]/[action]")]
    public class IdentityController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.configuration = configuration;
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
                "Account",
                new { code },
                this.Request.Scheme);

            await this.emailSender.SendEmailAsync(
                this.GetFromEmail(),
                "Bookworm",
                model.Email,
                user.UserName,
                "Password reset",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                this.GetAppPassword());

            this.TempData[TempDataMessageConstant.SuccessMessage] = "Please check your email to reset your password.";
            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
            => code == null ?
                this.BadRequest("A code must be supplied for password reset.") :
                this.View(new ResetPasswordInputModel { Code = code });

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordInputModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                this.TempData[TempDataMessageConstant.ErrorMessage] = ResetPasswordError;
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
                this.TempData[TempDataMessageConstant.ErrorMessage] = ConfirmEmailError;
                return this.RedirectToAction("Index", "Home", new { area = string.Empty });
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await this.userManager.ConfirmEmailAsync(user, code);

            this.ViewData["Message"] = result.Succeeded ?
                "Thank you for confirming your email." :
                "Error confirming your email.";

            return this.View();
        }

        private string GetFromEmail() => this.configuration.GetValue<string>("MailKitEmailSender:Email");

        private string GetAppPassword() => this.configuration.GetValue<string>("MailKitEmailSender:AppPassword");
    }
}
