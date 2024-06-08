namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Constants.DataConstants.CommentDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class Comment : BaseDeletableModel<int>
    {
        public Comment()
        {
            this.Votes = new HashSet<Vote>();
        }

        [Required]
        [MaxLength(CommentContentMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string Content { get; set; }

        [Required]
        public int NetWorth { get; set; }

        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Vote> Votes { get; set; }
    }
}
