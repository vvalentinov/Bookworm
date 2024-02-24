namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class QuoteLike : BaseDeletableModel<int>
    {
        [Required]
        [ForeignKey(nameof(Quote))]
        public int QuoteId { get; set; }

        public Quote Quote { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
