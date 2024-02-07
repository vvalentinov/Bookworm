namespace Bookworm.Web.ViewModels.Quotes.EditQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public abstract class BaseEditQuoteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = QuoteContentRequiredError)]
        [StringLength(
            QuoteContentMaxLength,
            MinimumLength = QuoteContentMinLength,
            ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }
    }
}
