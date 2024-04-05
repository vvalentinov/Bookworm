namespace Bookworm.Web.ViewModels.Quotes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Quotes.QuotesDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class UploadQuoteViewModel
        : IValidatableObject, IMapTo<QuoteDto>, IMapFrom<Quote>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = QuoteContentRequiredError)]
        [StringLength(
            QuoteContentMaxLength,
            MinimumLength = QuoteContentMinLength,
            ErrorMessage = QuoteContentLengthError)]
        public string Content { get; set; }

        public string AuthorName { get; set; }

        public string MovieTitle { get; set; }

        public string BookTitle { get; set; }

        public QuoteType Type { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Type == QuoteType.BookQuote)
            {
                if (string.IsNullOrWhiteSpace(this.AuthorName))
                {
                    yield return new ValidationResult(QuoteAuthorNameRequiredError);
                }

                if (string.IsNullOrWhiteSpace(this.BookTitle))
                {
                    yield return new ValidationResult(QuoteBookTitleRequiredError);
                }

                if (this.AuthorName?.Length < QuoteAuthorNameMinLength || this.AuthorName?.Length > QuoteAuthorNameMaxLength)
                {
                    yield return new ValidationResult(QuoteAuthorNameLengthError);
                }

                if (this.BookTitle?.Length < QuoteBookTitleMinLength || this.BookTitle?.Length > QuoteBookTitleMaxLength)
                {
                    yield return new ValidationResult(QuoteBookTitleLengthError);
                }
            }
            else if (this.Type == QuoteType.MovieQuote)
            {
                if (string.IsNullOrWhiteSpace(this.MovieTitle))
                {
                    yield return new ValidationResult(QuoteMovieTitleRequiredError);
                }

                if (this.MovieTitle?.Length < QuoteMovieTitleMinLength || this.MovieTitle?.Length > QuoteMovieTitleMaxLength)
                {
                    yield return new ValidationResult(QuoteMovieTitleLengthError);
                }
            }
            else if (this.Type == QuoteType.GeneralQuote)
            {
                if (string.IsNullOrWhiteSpace(this.AuthorName))
                {
                    yield return new ValidationResult(QuoteAuthorNameRequiredError);
                }

                if (this.AuthorName?.Length < QuoteAuthorNameMinLength || this.AuthorName?.Length > QuoteAuthorNameMaxLength)
                {
                    yield return new ValidationResult(QuoteAuthorNameLengthError);
                }
            }
            else
            {
                yield return new ValidationResult(QuoteInvalidTypeError);
            }
        }
    }
}
