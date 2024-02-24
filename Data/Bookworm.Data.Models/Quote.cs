namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class Quote : BaseDeletableModel<int>
    {
        [Required]
        [StringLength(
            QuoteContentMaxLength,
            MinimumLength = QuoteContentMinLength,
            ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public string AuthorName { get; set; }

        public string BookTitle { get; set; }

        public string MovieTitle { get; set; }

        [Required]
        public int Likes { get; set; }

        [Required]
        public QuoteType Type { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
