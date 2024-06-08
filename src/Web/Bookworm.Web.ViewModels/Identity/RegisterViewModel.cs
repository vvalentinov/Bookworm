namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.ViewModels.Identity.Base;

    public class RegisterViewModel : BaseAuthenticationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(
            "Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
