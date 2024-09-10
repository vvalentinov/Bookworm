namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    public class Category : BaseModel<int>
    {
        [Required]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
             = new HashSet<Book>();
    }
}
