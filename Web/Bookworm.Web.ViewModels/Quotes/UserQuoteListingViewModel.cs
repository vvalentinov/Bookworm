﻿namespace Bookworm.Web.ViewModels.Quotes
{
    using System.Collections.Generic;

    public class UserQuoteListingViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }

        public int ApprovedQuotesCount { get; set; }

        public int UnapprovedQuotesCount { get; set; }
    }
}