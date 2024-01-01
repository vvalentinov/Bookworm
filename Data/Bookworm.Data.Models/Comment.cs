namespace Bookworm.Data.Models
{
    using System.Collections.Generic;

    using Bookworm.Data.Common.Models;

    public class Comment : BaseModel<int>
    {
        public Comment()
        {
            this.Votes = new HashSet<Vote>();
        }

        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}
