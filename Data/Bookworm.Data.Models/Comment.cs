namespace Bookworm.Data.Models
{
    using Bookworm.Data.Common.Models;

    public class Comment : BaseDeletableModel<int>
    {
        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
