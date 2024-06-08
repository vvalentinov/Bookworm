namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class EmailInputViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
