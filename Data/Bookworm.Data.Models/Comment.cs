namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Comments.CommentsDataConstants;
    using static Bookworm.Common.Comments.CommentsErrorMessagesConstants;

    public class Comment : BaseDeletableModel<int>
    {
        public Comment()
        {
            this.Votes = new HashSet<Vote>();
        }

        [Required]
        [StringLength(
            CommentContentMaxLength,
            MinimumLength = CommentContentMinLength,
            ErrorMessage = CommentContentLengthError)]
        public string Content { get; set; }

        [Required]
        public int NetWorth { get; set; }

        [Required]
        [ForeignKey(nameof(Book))]
        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}
