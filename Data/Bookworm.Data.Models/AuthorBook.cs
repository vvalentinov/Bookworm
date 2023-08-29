namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class AuthorBook : BaseModel<int>
    {
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        [ForeignKey(nameof(Book))]
        public string BookId { get; set; }

        public virtual Book Book { get; set; }
    }
}
