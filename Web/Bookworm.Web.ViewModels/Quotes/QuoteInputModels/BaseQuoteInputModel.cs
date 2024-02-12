namespace Bookworm.Web.ViewModels.Quotes.QuoteInputModels
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public abstract class BaseQuoteInputModel : IMapTo<QuoteDto>, IMapFrom<Quote>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = QuoteContentRequiredError)]
        [StringLength(QuoteContentMaxLength, MinimumLength = QuoteContentMinLength, ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }
    }
}
