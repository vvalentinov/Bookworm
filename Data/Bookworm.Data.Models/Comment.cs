namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    public class Comment : BaseModel<int>
    {
        public Comment()
        {
            this.Votes = new HashSet<Vote>();
        }

        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        [StringLength(1000, MinimumLength = 20, ErrorMessage = "Comment's content must be between 20 and 1000 characters long!")]
        public string Content { get; set; }

        public int NetWorth { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}
