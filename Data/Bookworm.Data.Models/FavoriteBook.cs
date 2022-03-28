namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class FavoriteBook : BaseModel<int>
    {
        [ForeignKey(nameof(Book))]
        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
