namespace Bookworm.Web.ViewModels.Quotes.Models.UploadQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class UploadMovieQuoteViewModel
    {
        [Required(ErrorMessage = QuoteContentRequiredError)]
        [StringLength(QuoteContentMaxLength, MinimumLength = QuoteContentMinLength, ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }

        [Required(ErrorMessage = QuoteMovieTitleRequiredError)]
        [StringLength(QuoteMovieTitleMaxLength, MinimumLength = QuoteMovieTitleMinLength, ErrorMessage = QuoteMovieTitleLengthError)]
        public string MovieTitle { get; set; }
    }
}
