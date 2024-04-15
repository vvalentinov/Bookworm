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
        [StringLength(
            QuoteContentMaxLength,
            MinimumLength = QuoteContentMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Content { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public string AuthorName { get; set; }

        public string BookTitle { get; set; }

        public string MovieTitle { get; set; }

        [Required]
        public QuoteType Type { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
