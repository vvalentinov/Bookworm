namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;

    public class SearchQuoteService : ISearchQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public SearchQuoteService(IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteRepository = quoteRepository;
        }

        public List<QuoteViewModel> SearchQuoteByContent(
            string content,
            string userId,
            QuoteType? type = null)
        {
            List<QuoteViewModel> quotes = this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.Content.Contains(content) && (userId == null || x.UserId == userId))
                .OrderByDescending(x => x.CreatedOn)
                .To<QuoteViewModel>()
                .ToList();

            switch (type)
            {
                case QuoteType.ApprovedQuote:
                    return quotes.Where(q => q.IsApproved).ToList();
                case QuoteType.UnapprovedQuote:
                    return quotes.Where(q => q.IsApproved == false).ToList();
                case QuoteType.MovieQuote:
                    return quotes.Where(q => q.MovieTitle != null).ToList();
                case QuoteType.BookQuote:
                    return quotes.Where(q => q.BookTitle != null).ToList();
                case QuoteType.GeneralQuote:
                    return quotes.Where(q => q.BookTitle == null && q.MovieTitle == null).ToList();
                default: return quotes;
            }
        }
    }
}
