namespace Bookworm.Data.Models.DTOs
{
    using Bookworm.Data.Models.Enums;

    public class QuoteDto
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string AuthorName { get; set; }

        public string BookTitle { get; set; }

        public string MovieTitle { get; set; }

        public QuoteType Type { get; set; }
    }
}
