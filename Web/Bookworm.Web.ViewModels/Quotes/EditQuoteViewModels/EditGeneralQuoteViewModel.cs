namespace Bookworm.Web.ViewModels.Quotes.EditQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class EditGeneralQuoteViewModel : BaseEditQuoteViewModel, IMapFrom<Quote>
    {
        [Required(ErrorMessage = QuoteAuthorNameRequiredError)]
        [StringLength(
           QuoteAuthorNameMaxLength,
           MinimumLength = QuoteAuthorNameMinLength,
           ErrorMessage = QuoteAuthorNameLengthError)]
        public string AuthorName { get; set; }
    }
}
