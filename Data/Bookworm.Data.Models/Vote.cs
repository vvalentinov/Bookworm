namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;
    using Bookworm.Data.Models.Enums;

    public class Vote : BaseModel<int>
    {
        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public VoteValue Value { get; set; }
    }
}
