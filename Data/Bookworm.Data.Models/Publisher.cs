namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Publishers.PublishersDataConstants;
    using static Bookworm.Common.Publishers.PublishersErrorMessagesConstants;

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
            ErrorMessage = PublisherNameLengthError)]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
