namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
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
            var quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.CreatedOn)
                .To<QuoteViewModel>()
                .ToListAsync();

            return new QuoteListingViewModel { Quotes = await this.RetrieveQuoteUserStatusAsync(quotes, userId) };
        }

        public async Task<QuoteListingViewModel> GetAllApprovedQuotesAsync()
        {
            var quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved)
              .OrderBy(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel { Quotes = quotes };
        }

        public async Task<QuoteListingViewModel> GetAllUnapprovedQuotesAsync()
        {
            var quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false)
              .OrderBy(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel { Quotes = quotes };
        }

        public async Task<int> GetUnapprovedQuotesCountAsync() =>
            await this.quoteRepository
                .AllAsNoTracking()
                .Where(quote => quote.IsApproved == false)
                .CountAsync();

        public async Task<QuoteListingViewModel> GetAllDeletedQuotesAsync()
        {
            var quotes = await this.quoteRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .OrderBy(x => x.CreatedOn)
                .To<QuoteViewModel>()
                .ToListAsync();

            return new QuoteListingViewModel { Quotes = quotes };
        }

        public async Task<QuoteViewModel> GetQuoteByIdAsync(int quoteId)
        {
            var quote = await this.quoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            return new QuoteViewModel
            {
                Id = quoteId,
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                MovieTitle = quote.MovieTitle,
            };
        }

        public async Task<T> GetRandomQuoteAsync<T>() =>
            await this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefaultAsync();

        public async Task<List<QuoteViewModel>> GetLikedQuotesAsync(
            string userId,
            string sortQuotesCriteria,
            string content)
        {
            bool isValidSortCriteria = Enum.TryParse(sortQuotesCriteria, out SortQuotesCriteria sortCriteria);
            if (isValidSortCriteria == false)
            {
                throw new InvalidOperationException("Invalid sort quote criteria!");
            }

            var likedQuotesQuery = this.quoteRepository
                .AllAsNoTracking()
                .Where(quote => quote.IsApproved && quote.Likes > 0)
                .To<QuoteViewModel>();

            switch (sortCriteria)
            {
                case SortQuotesCriteria.NewestToOldest:
                    likedQuotesQuery = likedQuotesQuery.OrderByDescending(q => q.CreatedOn);
                    break;
                case SortQuotesCriteria.OldestToNewest:
                    likedQuotesQuery = likedQuotesQuery.OrderBy(q => q.CreatedOn);
                    break;
                case SortQuotesCriteria.LikesCountDesc:
                    likedQuotesQuery = likedQuotesQuery.OrderByDescending(q => q.Likes);
                    break;
            }

            var quotes = await likedQuotesQuery.ToListAsync();

            return await this.RetrieveQuoteUserStatusAsync(quotes, userId);
        }

        public async Task<List<QuoteViewModel>> GetAllQuotesByTypeAsync(
            string sortCriteria,
            string userId,
            string type,
            string content)
        {
            bool isValidSortCriteria = Enum.TryParse(sortCriteria, out SortQuotesCriteria sortQuotesCriteria);
            if (isValidSortCriteria == false)
            {
                throw new InvalidOperationException("Invalid sort quote criteria!");
            }

            var quotesQuery = this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved)
                .To<QuoteViewModel>();

            if (type != null)
            {
                bool isValidQuoteType = Enum.TryParse(type, out QuoteType quoteType);
                if (isValidQuoteType == false)
                {
                    throw new InvalidOperationException("Invalid quote type!");
                }

                quotesQuery = quotesQuery.Where(q => q.Type == quoteType);
            }

            if (string.IsNullOrWhiteSpace(content) == false)
            {
                quotesQuery = quotesQuery.Where(q => q.Content.ToLower().Contains(content.ToLower()));
            }

            switch (sortQuotesCriteria)
            {
                case SortQuotesCriteria.NewestToOldest:
                    quotesQuery = quotesQuery.OrderByDescending(q => q.CreatedOn);
                    break;
                case SortQuotesCriteria.OldestToNewest:
                    quotesQuery = quotesQuery.OrderBy(q => q.CreatedOn);
                    break;
                case SortQuotesCriteria.LikesCountDesc:
                    quotesQuery = quotesQuery.OrderByDescending(q => q.Likes);
                    break;
            }

            var quotes = await quotesQuery.ToListAsync();

            return await this.RetrieveQuoteUserStatusAsync(quotes, userId);
        }

        private async Task<List<QuoteViewModel>> RetrieveQuoteUserStatusAsync(List<QuoteViewModel> quotes, string userId)
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
