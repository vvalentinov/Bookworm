namespace Bookworm.Web.ViewModels.Quotes.QuoteInputModels
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class GeneralQuoteInputModel : BaseQuoteInputModel
    {
        [Required(ErrorMessage = QuoteAuthorNameRequiredError)]
        [StringLength(QuoteAuthorNameMaxLength, MinimumLength = QuoteAuthorNameMinLength, ErrorMessage = QuoteAuthorNameLengthError)]
        public string AuthorName { get; set; }
    }
}
