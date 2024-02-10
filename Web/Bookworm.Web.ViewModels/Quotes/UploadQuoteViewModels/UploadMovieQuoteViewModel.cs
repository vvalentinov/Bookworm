namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class UploadMovieQuoteViewModel : BaseUploadQuoteViewModel
    {
        [Required(ErrorMessage = QuoteMovieTitleRequiredError)]
        [StringLength(QuoteMovieTitleMaxLength, MinimumLength = QuoteMovieTitleMinLength, ErrorMessage = QuoteMovieTitleLengthError)]
        public string MovieTitle { get; set; }
    }
}
