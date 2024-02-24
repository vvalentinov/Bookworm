namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Ratings.RatingsDataConstants;
    using static Bookworm.Common.Ratings.RatingsErrorMessagesConstants;

    public class Rating : BaseModel<int>
    {
        [Range(
            RatingValueMin,
            RatingValueMax,
            ErrorMessage = RatingValueRangeError)]
        public byte Value { get; set; }

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
