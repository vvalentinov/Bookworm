namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;
    using Bookworm.Data.Models.Enums;

    public class Vote : BaseModel<int>
    {
        [Required]
        public VoteValue Value { get; set; }

        [Required]
        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }

        public Comment Comment { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
