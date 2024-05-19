namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class Notification : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(300, ErrorMessage = FieldMaxLengthError)]
        public string Content { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
