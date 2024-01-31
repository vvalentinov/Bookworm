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
    using Bookworm.Web.ViewModels.Quotes;
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

        public async Task<List<T>> SearchUserQuotesByContentAndTypeAsync<T>(
            string content,
            string userId,
            QuoteType type)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.UserId == userId &&
                       q.Content.ToLower() == content.ToLower() &&
                       q.Type == type)
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        public async Task<List<T>> SearchUserQuotesByContentAsync<T>(string content, string userId)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.UserId == userId && q.Content.ToLower() == content.ToLower())
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        public async Task<List<QuoteViewModel>> SearchQuotesByContentAndTypeAsync(string content, QuoteType type, string userId)
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved && q.Content.ToLower().Contains(content.ToLower()) && q.Type == type)
                .To<QuoteViewModel>()
                .ToListAsync();

            foreach (QuoteViewModel quote in quotes)
            {
                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);
                quote.IsUserQuoteCreator = quote.UserId == userId;
            }

            return quotes;
        }

        public async Task<List<QuoteViewModel>> SearchQuotesByContentAsync(string content, string userId)
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved && q.Content.ToLower().Contains(content.ToLower()))
                .To<QuoteViewModel>()
                .ToListAsync();

            foreach (QuoteViewModel quote in quotes)
            {
                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);
                quote.IsUserQuoteCreator = quote.UserId == userId;
            }

            return quotes;
        }

        public async Task<List<T>> SearchUserLikedQuotesByContentAsync<T>(string content, string userId)
        {
            List<int> likedQuotesIds = await this.quoteLikesRepository
                .AllAsNoTracking()
                .Where(q => q.UserId == userId)
                .Select(q => q.QuoteId)
                .ToListAsync();

            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => likedQuotesIds.Contains(q.Id) && q.Content.ToLower() == content.ToLower())
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        public async Task<List<T>> SearchLikedQuotesByContentAsync<T>(string content)
        {
            List<int> likedQuotesIds = await this.quoteLikesRepository
                .AllAsNoTracking()
                .Select(q => q.QuoteId)
                .ToListAsync();

            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => likedQuotesIds.Contains(q.Id) && q.Content.ToLower() == content.ToLower())
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        public async Task<List<T>> SearchUserApprovedQuotesByContentAsync<T>(string content, string userId)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.UserId == userId &&
                            q.IsApproved &&
                            q.Content.ToLower() == content.ToLower())
                .To<T>()
                .ToListAsync();

            return quotes;
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

        public async Task<List<T>> SearchUserUnapprovedQuotesByContentAsync<T>(string content, string userId)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.UserId == userId &&
                            q.IsApproved == false &&
                            q.Content.ToLower() == content.ToLower())
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
    }
}
