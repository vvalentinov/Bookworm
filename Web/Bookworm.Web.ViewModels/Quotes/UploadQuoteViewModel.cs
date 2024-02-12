namespace Bookworm.Web.ViewModels.Quotes
{
    using Bookworm.Web.ViewModels.Quotes.QuoteInputModels;

    public class UploadQuoteViewModel
    {
        public GeneralQuoteInputModel GeneralQuoteModel { get; set; }

        public MovieQuoteInputModel MovieQuoteModel { get; set; }

        public BookQuoteInputModel BookQuoteModel { get; set; }
    }
}
