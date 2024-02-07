namespace Bookworm.Web.ViewModels.Quotes.EditQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class EditMovieQuoteViewModel : BaseEditQuoteViewModel, IMapFrom<Quote>
    {
        [Required(ErrorMessage = QuoteMovieTitleRequiredError)]
        [StringLength(
            QuoteMovieTitleMaxLength,
            MinimumLength = QuoteMovieTitleMinLength,
            ErrorMessage = QuoteMovieTitleLengthError)]
        public string MovieTitle { get; set; }
    }
}
