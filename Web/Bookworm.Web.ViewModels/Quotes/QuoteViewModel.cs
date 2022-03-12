namespace Bookworm.Web.ViewModels.Quotes
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.DataConstants;
    using static Bookworm.Common.ErrorMessages;

    public class QuoteViewModel : IMapFrom<Quote>
    {
        [Required(ErrorMessage = QuoteContentRequired)]
        [StringLength(QuoteMaxLength, MinimumLength = QuoteMinLength, ErrorMessage = QuoteLength)]
        public string Content { get; set; }

        [StringLength(AuthorNameMax, MinimumLength = AuthorNameMin, ErrorMessage = AuthorNameLength)]
        public string AuthorName { get; set; }

        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLenght)]
        public string BookTitle { get; set; }

        [StringLength(MovieTitleMaxLenght, MinimumLength = MovieTitleMinLenght, ErrorMessage = MovieTitleLenght)]
        public string MovieTitle { get; set; }
    }
}
