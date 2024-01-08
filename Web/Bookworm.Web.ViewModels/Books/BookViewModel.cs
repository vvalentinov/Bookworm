namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Comments;
    using Ganss.Xss;

    public class BookViewModel : IMapFrom<Book>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string SanitizedDescription => new HtmlSanitizer().Sanitize(this.Description);

        public string PublisherName { get; set; }

        public int Year { get; set; }

        public double RatingsAvg { get; set; }

        public int RatingsCount { get; set; }

        public string UserId { get; set; }

        public int? UserRating { get; set; }

        public int DownloadsCount { get; set; }

        public int PagesCount { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsUserBook { get; set; }

        public string FileUrl { get; set; }

        public string CategoryName { get; set; }

        public string Username { get; set; }

        public PostCommentInputModel PostComment { get; set; }

        public IEnumerable<string> Authors { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
