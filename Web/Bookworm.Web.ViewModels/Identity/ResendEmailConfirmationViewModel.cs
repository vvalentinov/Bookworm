namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class ResendEmailConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
