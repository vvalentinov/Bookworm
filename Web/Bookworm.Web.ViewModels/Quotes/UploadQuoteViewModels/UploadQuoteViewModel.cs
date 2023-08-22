﻿namespace Bookworm.Web.ViewModels.Quotes.UploadQuoteViewModels
{
    public class UploadQuoteViewModel
    {
        public UploadGeneralQuoteViewModel GeneralQuoteModel { get; set; }

        public UploadMovieQuoteViewModel MovieQuoteModel { get; set; }

        public UploadBookQuoteViewModel BookQuoteModel { get; set; }

        public string MovieQuoteImgUrl { get; set; }

        public string BookQuoteImgUrl { get; set; }

        public string GeneralQuoteImgUrl { get; set; }
    }
}