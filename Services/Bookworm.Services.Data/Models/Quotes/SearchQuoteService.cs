namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes.Models;
    using Microsoft.EntityFrameworkCore;

    public class SearchQuoteService : ISearchQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IRepository<QuoteLike> quoteLikesRepository;

        public SearchQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IRepository<QuoteLike> quoteLikesRepository)
        {
            this.quoteRepository = quoteRepository;
            this.quoteLikesRepository = quoteLikesRepository;
        }

        public async Task<List<QuoteViewModel>> SearchQuotesByContentAndTypeAsync(
            string content,
            QuoteType type,
            string userId)
        {
            var quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q =>
                       q.IsApproved &&
                       q.Content.ToLower().Contains(content.ToLower()) &&
                       q.Type == type)
                .To<QuoteViewModel>()
                .ToListAsync();

            return await this.RetrieveQuoteUserStatusAsync(quotes, userId);
        }

        public async Task<List<QuoteViewModel>> SearchQuotesByContentAsync(string content, string userId)
        {
            var quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved && q.Content.ToLower().Contains(content.ToLower()))
                .To<QuoteViewModel>()
                .ToListAsync();

            return await this.RetrieveQuoteUserStatusAsync(quotes, userId);
        }

        public async Task<List<QuoteViewModel>> SearchLikedQuotesByContentAsync(string content, string userId)
        {
            var quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q =>
                       q.Likes > 0 &&
                       q.Content.ToLower().Contains(content.ToLower()))
                .To<QuoteViewModel>()
                .ToListAsync();

            return await this.RetrieveQuoteUserStatusAsync(quotes, userId);
        }

        public async Task<List<T>> SearchApprovedQuotesByContentAsync<T>(string content)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved && q.Content.ToLower() == content.ToLower())
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        public async Task<List<T>> SearchUnapprovedQuotesByContentAsync<T>(string content)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved == false && q.Content.ToLower() == content.ToLower())
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        private async Task<List<QuoteViewModel>> RetrieveQuoteUserStatusAsync(
            List<QuoteViewModel> quotes,
            string userId)
        {
            foreach (QuoteViewModel quote in quotes)
            {
                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);
                quote.IsUserQuoteCreator = quote.UserId == userId;
            }

            return quotes;
        }
    }
}
