namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class PublisherBook
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Publisher))]
        public int PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }

        [ForeignKey(nameof(Book))]
        public string BookId { get; set; }

        public virtual Book Book { get; set; }
    }
}
