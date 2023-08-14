namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class QuoteLike : BaseModel<int>
    {
        [Required]
        public int Likes { get; set; }

        [Required]
        [ForeignKey(nameof(Quote))]
        public int QuoteId { get; set; }

        public virtual Quote Quote { get; set; }
    }
}
