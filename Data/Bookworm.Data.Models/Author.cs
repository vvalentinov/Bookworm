namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Authors.AuthorsDataConstants;

    public class Author : BaseDeletableModel<int>
    {
        public Author()
        {
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }

        [Required]
        [MaxLength(AuthorNameMaxLength)]
        public string Name { get; set; }

        public virtual ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
