namespace Bookworm.Web.ViewModels.Quotes
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Data.Models.DTOs;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class QuoteInputModel : IMapFrom<Quote>, IMapTo<QuoteDto>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = QuoteContentRequiredError)]
        [StringLength(
            QuoteContentMaxLength,
            MinimumLength = QuoteContentMinLength,
            ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }

        public string AuthorName { get; set; }

        public string BookTitle { get; set; }

        public string MovieTitle { get; set; }

        public QuoteType? Type { get; set; }
    }
}
