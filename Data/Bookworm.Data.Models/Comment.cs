namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Comments.CommentsErrorMessagesConstants;

    public class Comment : BaseModel<int>
    {
        public Comment()
        {
            this.Votes = new HashSet<Vote>();
        }

        [ForeignKey(nameof(Book))]
        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        [StringLength(1000, MinimumLength = 20, ErrorMessage = CommentContentLengthError)]
        public string Content { get; set; }

        public int NetWorth { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}
