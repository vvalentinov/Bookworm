namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    public class Publisher : BaseDeletableModel<int>
    {
        public Publisher()
        {
            this.Books = new HashSet<Book>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
