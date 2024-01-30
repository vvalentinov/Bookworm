namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
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

    public class RetrieveQuotesService : IRetrieveQuotesService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IRepository<QuoteLike> quoteLikesRepository;

        public RetrieveQuotesService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IRepository<QuoteLike> quoteLikesRepository)
        {
            this.quoteRepository = quoteRepository;
            this.quoteLikesRepository = quoteLikesRepository;
        }

        public async Task<QuoteListingViewModel> GetAllQuotesAsync(string userId)
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .To<QuoteViewModel>()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            foreach (QuoteViewModel quote in quotes)
            {
                quote.Likes = await this.GetQuoteLikesAsync(quote.Id);
                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);
            }

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<QuoteListingViewModel> GetAllApprovedQuotesAsync()
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved)
              .OrderBy(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<QuoteListingViewModel> GetAllUnapprovedQuotesAsync()
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false)
              .OrderBy(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<int> GetUnapprovedQuotesCountAsync()
        {
            return await this.quoteRepository
                .AllAsNoTracking()
                .Where(quote => quote.IsApproved == false)
                .CountAsync();
        }

        public async Task<QuoteListingViewModel> GetAllDeletedQuotesAsync()
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .OrderBy(x => x.CreatedOn)
                .To<QuoteViewModel>()
                .ToListAsync();

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<QuoteViewModel> GetQuoteByIdAsync(int quoteId)
        {
            Quote quote = await this.quoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            return new QuoteViewModel()
            {
                Id = quoteId,
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                MovieTitle = quote.MovieTitle,
            };
        }

        public async Task<T> GetRandomQuoteAsync<T>()
        {
            return await this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetLikedQuotesAsync<T>()
        {
            List<int> likedQuotesIds = await this.quoteLikesRepository
                .AllAsNoTracking()
                .Select(quote => quote.Id)
                .ToListAsync();

            List<T> likedQuotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(quote => likedQuotesIds.Contains(quote.Id))
                .OrderByDescending(quote => quote.CreatedOn)
                .To<T>()
                .ToListAsync();

            return likedQuotes;
        }

        public async Task<List<T>> GetAllQuotesByTypeAsync<T>(QuoteType type)
        {
            List<T> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.Type == type)
                .To<T>()
                .ToListAsync();

            return quotes;
        }

        private async Task<int> GetQuoteLikesAsync(int quoteId)
        {
            return await this.quoteLikesRepository
                .AllAsNoTracking()
                .CountAsync(ql => ql.QuoteId == quoteId);
        }
    }
}
