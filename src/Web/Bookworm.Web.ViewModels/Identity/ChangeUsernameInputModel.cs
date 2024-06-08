namespace Bookworm.Web.ViewModels.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class ChangeUsernameInputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
    }
}
