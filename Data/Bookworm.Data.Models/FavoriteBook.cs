namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class FavoriteBook : BaseDeletableModel<int>
    {
        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
