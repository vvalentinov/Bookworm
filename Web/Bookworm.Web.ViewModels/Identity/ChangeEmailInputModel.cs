namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class ChangeEmailInputModel
    {
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}
