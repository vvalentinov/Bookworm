namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.ViewModels.Identity.Base;

    public class LoginViewModel : BaseAuthenticationViewModel
    {
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
