namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Authors.AuthorsDataConstants;
    using static Bookworm.Common.Authors.AuthorsErrorMessagesConstants;

    public class Author : BaseModel<int>
    {
        public Author()
        {
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }

        [Required]
        [StringLength(
            AuthorNameMaxLength,
            MinimumLength = AuthorNameMinLength,
            ErrorMessage = AuthorNameLengthError)]
        public string Name { get; set; }

        public virtual ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
