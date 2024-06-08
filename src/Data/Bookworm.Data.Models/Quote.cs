namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class Quote : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(QuoteContentMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string Content { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [MaxLength(QuoteSourceMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string AuthorName { get; set; }

        [MaxLength(QuoteSourceMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string BookTitle { get; set; }

        [MaxLength(QuoteSourceMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string MovieTitle { get; set; }

        [Required]
        public QuoteType Type { get; set; }

        [Required]
        public int Likes { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
