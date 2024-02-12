namespace Bookworm.Web.ViewModels.Quotes.QuoteInputModels
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class MovieQuoteInputModel : BaseQuoteInputModel
    {
        [Required(ErrorMessage = QuoteMovieTitleRequiredError)]
        [StringLength(QuoteMovieTitleMaxLength, MinimumLength = QuoteMovieTitleMinLength, ErrorMessage = QuoteMovieTitleLengthError)]
        public string MovieTitle { get; set; }
    }
}
