namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginInputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}
