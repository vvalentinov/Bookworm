namespace Bookworm.Services.Data.Contracts.Quotes
{
    public interface IGetQuoteTypeImgUrlService
    {
        string GetMovieQuoteImageUrl();

        string GetBookQuoteImageUrl();

        string GetGeneralQuoteImageUrl();
    }
}
