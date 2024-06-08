namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Constants.DataConstants.AuthorDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

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
            ErrorMessage = FieldStringLengthError)]
        public string Name { get; set; }

        public ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
