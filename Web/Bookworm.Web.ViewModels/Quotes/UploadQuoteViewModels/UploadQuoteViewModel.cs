namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Microsoft.Extensions.Configuration;

    public class UploadQuoteViewModel : IMapFrom<Quote>
    {
        private readonly IConfiguration configuration;

        public UploadQuoteViewModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public UploadGeneralQuoteViewModel GeneralQuoteModel => new UploadGeneralQuoteViewModel(this.configuration);

        public UploadMovieQuoteViewModel MovieQuoteModel => new UploadMovieQuoteViewModel(this.configuration);

        public UploadBookQuoteViewModel BookQuoteModel => new UploadBookQuoteViewModel(this.configuration);
    }
}
