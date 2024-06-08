namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Constants.DataConstants.PublisherDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class Publisher : BaseModel<int>
    {
        public Publisher()
        {
            this.Books = new HashSet<Book>();
        }

        [Required]
        [StringLength(
            PublisherNameMaxLength,
            MinimumLength = PublisherNameMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
