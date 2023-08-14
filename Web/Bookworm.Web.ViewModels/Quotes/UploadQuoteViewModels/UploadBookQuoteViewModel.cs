namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.DataConstants;
    using static Bookworm.Common.ErrorMessages;

    public class UploadBookQuoteViewModel : IMapFrom<Quote>
    {
        [Required(ErrorMessage = QuoteContentRequired)]
        [StringLength(QuoteMaxLength, MinimumLength = QuoteMinLength, ErrorMessage = QuoteLength)]
        public string Content { get; set; }

        [Required(ErrorMessage = QuoteBookTitleRequired)]
        [StringLength(QuoteBookTitleMaxLenght, MinimumLength = QuoteBookTitleMinLenght, ErrorMessage = QuoteBookTitleLength)]
        public string BookTitle { get; set; }

        [Required(ErrorMessage = AuthorNameRequired)]
        [StringLength(AuthorNameMax, MinimumLength = AuthorNameMin, ErrorMessage = AuthorNameLength)]
        public string AuthorName { get; set; }
    }
}
