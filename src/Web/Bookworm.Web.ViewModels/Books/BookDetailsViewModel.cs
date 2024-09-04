namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Comments;
    using Ganss.Xss;

    public class BookDetailsViewModel : BookViewModel
    {
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

        public bool IsFavorite { get; set; }

        public bool IsUserBook { get; set; }

        public string FileUrl { get; set; }

        public string CategoryName { get; set; }

        public string Username { get; set; }

        public bool IsApproved { get; set; }

        public PostCommentInputModel PostComment { get; set; }

        public IEnumerable<string> Authors { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }

        //public static BookDetailsViewModel MapFromBook(Book book)
        //{
        //    var authors = book
        //        .AuthorsBooks
        //        .Select(x => x.Author.Name)
        //        .ToList();

        //    return new BookDetailsViewModel
        //    {
        //        Authors = authors,
        //        CategoryName = book.Category.Name,
        //        Comments = book.Comments.Select(x => new CommentViewModel { }),

        //    };
        //};

        //public void CreateMappings(IProfileExpression configuration)
        //{
        //    configuration
        //        .CreateMap<Book, BookDetailsViewModel>()
        //        .ForMember(dest => dest.Language, opt => opt.MapFrom(b => b.Language.Name))
        //        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(b => b.Category.Name))
        //        .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(b => b.Publisher.Name))
        //        .ForMember(dest => dest.Authors, opt => opt.MapFrom(b => b.AuthorsBooks.Select(x => x.Author.Name)));
        //}
    }
}
