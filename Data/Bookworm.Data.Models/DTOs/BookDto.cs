namespace Bookworm.Data.Models
{
    using Microsoft.AspNetCore.Http;

    public class BookDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public int PagesCount { get; set; }

        public int PublishedYear { get; set; }

        public IFormFile BookFile { get; set; }

        public IFormFile ImageFile { get; set; }

        public int CategoryId { get; set; }

        public int LanguageId { get; set; }
    }
}
