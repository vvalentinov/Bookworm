namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class AuthorBook : BaseDeletableModel<int>
    {
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }

        public Author Author { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
