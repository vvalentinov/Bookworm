namespace Bookworm.Web.ViewModels.Quotes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.DTOs;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;
    using static Bookworm.Common.Enums.QuoteType;

    public class UploadQuoteViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = FieldRequiredError)]
        [StringLength(
            QuoteContentMaxLength,
            MinimumLength = QuoteContentMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Content { get; set; }

        public string AuthorName { get; set; }

        public string MovieTitle { get; set; }

        public string BookTitle { get; set; }

        public QuoteType Type { get; set; }

        public static UploadQuoteViewModel MapFromQuote(Quote quote)
        {
            return new UploadQuoteViewModel
            {
                Id = quote.Id,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                Content = quote.Content,
                MovieTitle = quote.MovieTitle,
                Type = quote.Type,
            };
        }

        public QuoteDto MapToQuoteDto()
        {
            return new QuoteDto
            {
                AuthorName = this.AuthorName,
                BookTitle = this.BookTitle,
                Content = this.Content,
                Id = this.Id,
                MovieTitle = this.MovieTitle,
                Type = this.Type,
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Type == BookQuote)
            {
                if (string.IsNullOrWhiteSpace(this.AuthorName))
                {
                    yield return new ValidationResult(string.Format(
                        FieldRequiredError,
                        "Author"));
                }

                if (string.IsNullOrWhiteSpace(this.BookTitle))
                {
                    yield return new ValidationResult(string.Format(
                        FieldRequiredError,
                        "Book Title"));
                }

                if (this.AuthorName?.Length < QuoteSourceMinLength ||
                    this.AuthorName?.Length > QuoteContentMaxLength)
                {
                    yield return new ValidationResult(string.Format(
                        FieldStringLengthError,
                        "Author",
                        QuoteContentMaxLength,
                        QuoteSourceMinLength));
                }

                if (this.BookTitle?.Length < QuoteSourceMinLength ||
                    this.BookTitle?.Length > QuoteContentMaxLength)
                {
                    yield return new ValidationResult(string.Format(
                        FieldStringLengthError,
                        "Book Title",
                        QuoteContentMaxLength,
                        QuoteSourceMinLength));
                }
            }
            else if (this.Type == MovieQuote)
            {
                if (string.IsNullOrWhiteSpace(this.MovieTitle))
                {
                    yield return new ValidationResult(string.Format(
                        FieldRequiredError,
                        "Movie Title"));
                }

                if (this.MovieTitle?.Length < QuoteSourceMinLength ||
                    this.MovieTitle?.Length > QuoteContentMaxLength)
                {
                    yield return new ValidationResult(string.Format(
                        FieldStringLengthError,
                        "Movie Title",
                        QuoteContentMaxLength,
                        QuoteSourceMinLength));
                }
            }
            else if (this.Type == GeneralQuote)
            {
                if (string.IsNullOrWhiteSpace(this.AuthorName))
                {
                    yield return new ValidationResult(string.Format(
                        FieldRequiredError,
                        "Author"));
                }

                if (this.AuthorName?.Length < QuoteSourceMinLength ||
                    this.AuthorName?.Length > QuoteContentMaxLength)
                {
                    yield return new ValidationResult(string.Format(
                        FieldStringLengthError,
                        "Author",
                        QuoteContentMaxLength,
                        QuoteSourceMinLength));
                }
            }
            else
            {
                yield return new ValidationResult(QuoteInvalidTypeError);
            }
        }
    }
}
