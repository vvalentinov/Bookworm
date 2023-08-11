namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Microsoft.Extensions.Configuration;

    using static Bookworm.Common.DataConstants;
    using static Bookworm.Common.ErrorMessages;

    public class UploadMovieQuoteViewModel : IMapFrom<Quote>
    {
        private readonly IConfiguration configuration;

        public UploadMovieQuoteViewModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Required(ErrorMessage = QuoteContentRequired)]
        [StringLength(QuoteMaxLength, MinimumLength = QuoteMinLength, ErrorMessage = QuoteLength)]
        public string Content { get; set; }

        [Required(ErrorMessage = MovieTitleRequired)]
        [StringLength(MovieTitleMaxLenght, MinimumLength = MovieTitleMinLenght, ErrorMessage = MovieTitleLength)]
        public string MovieTitle { get; set; }

        public string ImageUrl => this.configuration.GetValue<string>("QuotesImages:MovieQuotes");
    }
}
