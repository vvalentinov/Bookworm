namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.DataConstants;
    using static Bookworm.Common.ErrorMessages;

    public class UploadMovieQuoteViewModel : IMapFrom<Quote>
    {
        [Required(ErrorMessage = QuoteContentRequired)]
        [StringLength(QuoteMaxLength, MinimumLength = QuoteMinLength, ErrorMessage = QuoteLength)]
        public string Content { get; set; }

        [Required(ErrorMessage = MovieTitleRequired)]
        [StringLength(MovieTitleMaxLenght, MinimumLength = MovieTitleMinLenght, ErrorMessage = MovieTitleLength)]
        public string MovieTitle { get; set; }
    }
}
