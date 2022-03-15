namespace Bookworm.Web.ViewModels.Books
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    public class BookViewModel : IMapFrom<Book>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }

        public int DownloadsCount { get; set; }

        public int PagesCount { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public string FileUrl { get; set; }
    }
}
