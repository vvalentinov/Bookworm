namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class UploadBookQuoteViewModel
    {
        [Required(ErrorMessage = QuoteContentRequiredError)]
        [StringLength(QuoteContentMaxLength, MinimumLength = QuoteContentMinLength, ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }

        [Required(ErrorMessage = QuoteBookTitleRequiredError)]
        [StringLength(QuoteBookTitleMaxLength, MinimumLength = QuoteBookTitleMinLength, ErrorMessage = QuoteBookTitleLengthError)]
        public string BookTitle { get; set; }

        [Required(ErrorMessage = QuoteAuthorNameRequiredError)]
        [StringLength(QuoteAuthorNameMaxLength, MinimumLength = QuoteAuthorNameMinLength, ErrorMessage = QuoteAuthorNameLengthError)]
        public string AuthorName { get; set; }
    }
}
