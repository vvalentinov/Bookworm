namespace Bookworm.Services.Data.Models.Quotes
{
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.Extensions.Configuration;

    public class GetQuoteTypeImgUrlService : IGetQuoteTypeImgUrlService
    {
        private readonly IConfiguration configuration;

        public GetQuoteTypeImgUrlService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetBookQuoteImageUrl()
        {
            return this.configuration.GetValue<string>("QuotesImages:BookQuotes");
        }

        public string GetGeneralQuoteImageUrl()
        {
            return this.configuration.GetValue<string>("QuotesImages:GeneralQuotes");
        }

        public string GetMovieQuoteImageUrl()
        {
            return this.configuration.GetValue<string>("QuotesImages:MovieQuotes");
        }
    }
}
